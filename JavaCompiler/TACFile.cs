using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaCompiler
{
    public static class TACFile
    {
        private static List<string> lines { get; set; } = new List<string>();

        /// <summary>
        /// Adds the built code to the list of lines
        /// </summary>
        public static void AddLine(string line)
        {
            lines.Add(line);
        }

        /// <summary>
        /// Creates the TAC file once every line in the file has been read.
        /// </summary>
        public static void CreateTACFile()
        {
            string tacFileName = JavaFile.fileName.Split(".")[0] + ".TAC";

            using (StreamWriter sw = new StreamWriter(tacFileName))
            {
                foreach(string line in lines)
                {
                    Console.WriteLine(line);
                    sw.WriteLine(line);
                }
            }
        }
    }
}
