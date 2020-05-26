using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RegistrationSMEV
{
    class fileTxt
    {
        public static HashSet<string> StringList = new HashSet<string>();
        public static void Read(string str)
        {
            using (StreamReader fs = new StreamReader(@str, Encoding.Default))
            {
                while (true)
                {
                    string temp = fs.ReadLine();

                    if (string.IsNullOrWhiteSpace(temp))
                        break;

                    if(IsEmptyOrWhitespace(temp))
                        break;

                    if (!temp.Contains("ИНН"))
                        StringList.Add(temp);
                }
            }
        }

        static bool IsEmptyOrWhitespace(string s)
        {
            if (s == null || s.Length == 0) return true;
            for (int i = 0; i < s.Length; i++) if (!char.IsWhiteSpace(s[i])) return false;
            return true;
        }
    }
}
