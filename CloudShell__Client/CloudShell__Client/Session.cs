using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudShell__Client
{
    class Session
    {
        static private string[] Adjectives = {
            "agreeable",
            "brave",
            "calm",
            "delightful",
            "eager",
            "faithful",
            "gentle",
            "happy",
            "jolly",
            "kind",
            "lively",
            "nice",
            "obedient",
            "proud",
            "relieved",
            "silly",
            "thankful",
            "victorious",
            "witty",
            "zealous"
        };

        static private string[] Nouns = {
            "ant",
            "bear",
            "bee",
            "bird",
            "camel",
            "cat",
            "cheetah",
            "chicken",
            "cow"
        };

        public string Name
        {
            get
            {
                return "session_" + session_id + ".txt";
            }
        }

        private string session_id;
        private string input_directory_id;
        private string output_directory_id;

        public Session(string id, string in_dir_id, string out_dir_id)
        {
            session_id = id;
            input_directory_id = in_dir_id;
            output_directory_id = out_dir_id;
        }

        public Session(string in_dir_id, string out_dir_id)
        {
            Random random = new Random();
            string adjective = Adjectives[random.Next(0, Adjectives.Length)];
            string noun = Nouns[random.Next(0, Nouns.Length)];

            session_id = adjective + "_" + noun;

            input_directory_id = in_dir_id;
            output_directory_id = out_dir_id;
        }

        public Session(string session)
        {
            if (session.Length != 0)
            {
                string id = session.Split(':')[0];
                string in_dir_id = session.Split(':')[1];
                string out_dir_id = session.Split(':')[2];

                session_id = id;
                input_directory_id = in_dir_id;
                output_directory_id = out_dir_id;
            }
            else {

                Random random = new Random();
                string adjective = Adjectives[random.Next(0, Adjectives.Length)];
                string noun = Nouns[random.Next(0, Nouns.Length)];

                session_id = "bad_" + adjective + "_" + noun;
            }
        }
        public string InputFolder
        {
            get
            {
                return input_directory_id;
            }
        }

        public string OutputFolder
        {
            get
            {
                return output_directory_id;
            }
        }

        public string Identifier
        {
            get
            {
                return session_id;
            }
        }
        public override string ToString()
        {
            return session_id + ":" + input_directory_id + ":" + output_directory_id;
        }
    }
}
