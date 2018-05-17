using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudShell__Client
{
    interface ISessionManager__Client
    {
        void UploadRecord(Record record);
        List<Record> GetRecords();
    }
}
