using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudShell__Server
{
    enum RecordType
    {
        Command,
        CommandResult,
        UploadFile,
        UploadFileResult,
        DownloadFile,
        DownloadFileResult,
        LoadScript,
        LoadScriptResult,
    };

    class Record
    {
        public Guid Guid { get; set; }
        public RecordType Type { get; set; }
        public string Name
        {
            get
            {
                return "record_" + Guid.ToString();
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return Base64Decode(_content);
            }
            set
            {
                _content = Base64Encode(value);
            }
        }

        public override string ToString()
        {
            return Guid.ToString() + ":" + Type.ToString() + ":" + _content;
        }

        public Record() { }
        public Record(string record)
        {
            string guid = record.Split(':')[0];
            string type = record.Split(':')[1];
            string content = record.Split(':')[2];

            Guid = new Guid(guid);
            Type = (RecordType)Enum.Parse(typeof(RecordType), type);
            _content = content;
        }

        public Record(Guid guid, RecordType type, string content)
        {
            Guid = guid;
            Type = type;
            Content = content;
        }

        public static string Base64Encode_b(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static byte[] Base64Decode_b(string base64EncodedData)
        {
            byte[] base64DecodedBytes = Convert.FromBase64String(base64EncodedData);
            return base64DecodedBytes;
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
