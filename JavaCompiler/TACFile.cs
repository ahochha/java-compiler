using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaCompiler
{
    public static class TACFile
    {
        private static List<string> lines { get; set; } = new List<string>();
        private static StreamWriter streamWriter { get; set; }

        public static void AddLine(string line)
        {
            lines.Add(line);
        }

        public static void CreateTACFile()
        {
            string tacFileName = JavaFile.fileName.Split(".")[0] + ".TAC";

            using (StreamWriter sw = new StreamWriter(tacFileName))
            {
                foreach(string line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        public static string GetFinalVarName()
        {
            return lines[lines.Count - 1].Split("=")[0].Trim();
        }
    }
}
