using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;



namespace CloudShell__Server
{
    class Program
    {
        private static string serviceAccountEmail = "";
        private static string p12Base64 = "";
        static void Main(string[] args)
        {
            ISessionManager__Server sm = new GoogleDriveSessionManager__Server(serviceAccountEmail, p12Base64);
            CommandHandler ch = new CommandHandler(sm);

            while (true)
            {
                ch.HandleCommand();
            }
        }
    }
}
