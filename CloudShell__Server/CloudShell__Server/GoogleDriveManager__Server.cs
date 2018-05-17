using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace CloudShell__Server
{
    class GoogleDriveManager__Server
    {
        private DriveService drive;
        public GoogleDriveManager__Server(string serviceAccountEmail, string certificateBase64)
        {
            string[] scopes = new string[] { DriveService.Scope.Drive };
            var certificate = new X509Certificate2(Convert.FromBase64String(certificateBase64), "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = scopes
                }.FromCertificate(certificate)
            );

            drive = new DriveService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CloudShell"
                }
            );
        }

        public File CreateFile(File file, string data)
        {
			file.MimeType = "text/plain";
            byte[] byteArray = Encoding.ASCII.GetBytes(data);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            FilesResource.InsertMediaUpload request = drive.Files.Insert(file, stream, "text/plain");
            request.Upload();

            return request.ResponseBody;
        }
        public File CreateDirectory(File directory)
        {
            File dir = drive.Files.Insert(directory).Execute();
            return dir;
        }
        public void DeleteFile(File file)
        {
            drive.Files.Delete(file.Id).Execute();
        }
        public string GetFileContent(File file)
        {
            var request = drive.Files.Get(file.Id);
            var stream = new System.IO.MemoryStream();

            request.Download(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        //stream DownloadFile(string file_id);


        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public List<File> ListFiles(string parent_id, string mime = "text/plain")
        {
            List<File> files = ListItems(parent_id, mime);
            return files;
        }
        public List<File> ListDirectories(string parent_id)
        {
            List<File> directories = ListItems(parent_id, "application/vnd.google-apps.folder");
            return directories;
        }

        private List<File> ListItems(string parent_id, string mime)
        {
            List<File> files = new List<File>();

            FilesResource.ListRequest listRequest = drive.Files.List();
            listRequest.Q = "'" + parent_id + "' in parents";

            FileList file_list = listRequest.Execute();
            foreach (var file in file_list.Items)
            {
                if (file.MimeType == mime)
                    files.Add(file);
            }

            return files;
        }

        public void PurgeAll()
        {
            List<File> files = new List<File>();

            FilesResource.ListRequest listRequest = drive.Files.List();
            listRequest.Q = "'root' in parents";

            FileList file_list = listRequest.Execute();
            foreach (var file in file_list.Items)
            {
                DeleteFile(file);
            }
        }
    }
}
