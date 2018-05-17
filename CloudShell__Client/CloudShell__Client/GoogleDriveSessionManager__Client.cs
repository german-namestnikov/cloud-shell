using Google.Apis.Drive.v2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudShell__Client
{
    class GoogleDriveSessionManager__Client : ISessionManager__Client
    {
        private Session current_session;
        private GoogleDriveManager__Client gd_manager;
        public GoogleDriveSessionManager__Client(string serviceAccountEmail, string certificateBase64)
        {
            gd_manager = new GoogleDriveManager__Client(serviceAccountEmail, certificateBase64);
            InitSession();
        }
        void InitSession()
        {
            File file = new File();
            file.Description = "InputFolder";
            int r = (new Random()).Next(100000, 999999);
            file.Title = "input_" + Convert.ToString(r);
            file.Parents = new List<ParentReference>() { new ParentReference() { Id = "root" } };

            string input_id = gd_manager.CreateDirectory(file).Id;

            file.Description = "OutputFolder";
            file.Title = "output_" + Convert.ToString(r);

            string output_id = gd_manager.CreateDirectory(file).Id;

            current_session = new Session(input_id, output_id);

            file.Description = "Session";
            file.Title = current_session.Name;
            file.Parents = new List<ParentReference>() { new ParentReference() { Id = "root" } };

            gd_manager.CreateFile(file, current_session.ToString());
        }
        public void UploadRecord(Record record)
        {
            File file = new File();
            file.Title = record.Name;
            file.Description = "Test";
            file.Parents = new List<ParentReference>() { new ParentReference() { Id = current_session.OutputFolder } };

            gd_manager.CreateFile(file, record.ToString());
        }
        public List<Record> GetRecords()
        {
            List<Record> records = new List<Record>();
            List<File> files = gd_manager.ListFiles(current_session.InputFolder);
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
    }
}
