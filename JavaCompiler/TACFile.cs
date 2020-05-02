using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public static class TACFile
    {
        public static string tacFileName { get; set; }
        public static string tacWord { get; set; }
        private static StreamReader program { get; set; }
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
            tacFileName = JavaFile.fileName.Split(".")[0] + ".TAC";

            using (StreamWriter sw = new StreamWriter(tacFileName))
            {
                foreach(string line in lines)
                {
                    Console.WriteLine(line);
                    sw.WriteLine(line);
                }
            }
        }

        public static void ReadLinesFromFile()
        {
            string filePath = $@"{Environment.CurrentDirectory}\\" + tacFileName;

            program = File.OpenText(filePath);
        }

        public static string GetNextWord()
        {
            tacWord = "";

            while (char.IsWhiteSpace(CurrentChar))
            {
                CurrentChar = (char)program.Read();
            }

            while (!char.IsWhiteSpace(CurrentChar))
            {
                tacWord += CurrentChar;
                CurrentChar = (char)program.Read();
            }

            return tacWord;
        }

        public static char PeekNextChar()
        {
            while (char.IsWhiteSpace((char)program.Peek()))
            {
                CurrentChar = (char)program.Read();
            }

            return (char)program.Peek();
        }
    }
}
