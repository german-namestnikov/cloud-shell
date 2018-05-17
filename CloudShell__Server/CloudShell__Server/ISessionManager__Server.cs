using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudShell__Server
{
    interface ISessionManager__Server
    {
        List<string> GetSessions();
        void SetCurrentSession(string session_id);
        string GetCurrentSession();
        void UploadRecord(Record record);
        List<Record> GetRecords();
        void Purge();
    }
}
