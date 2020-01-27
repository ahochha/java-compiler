using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JavaCompiler
{
    public static class FileHandler
    {
        public static List<string> Lines { get; set; } = new List<string>();
        public static string CurrentLine { get; set; }
        public static int CharNum { get; set; }

        public static void ReadLines(string filePath)
        {
            Lines = File.ReadAllLines(filePath).ToList();
            CurrentLine = Lines[0];
        }
    }
}
