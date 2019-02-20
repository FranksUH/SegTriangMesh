using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TMesh_2._0
{
    public class FilesChat
    {
        public static void SendAndOverWrite(string chanel, string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(chanel + ".txt");
                sw.WriteLine(message);
                sw.Close();
            }
            catch
            { }
        }
        public static string ReadLine(string chanel)
        {
            try
            {
                StreamReader sr = new StreamReader(chanel + ".txt");
                string result = sr.ReadLine();
                sr.Close();
                return result;
            }
            catch
            {
                return "-1";
            }
        }
    }
}
