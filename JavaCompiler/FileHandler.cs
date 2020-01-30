using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JavaCompiler
{
    public class FileHandler
    {
        public List<string> lines { get; set; }
        public string currentLine { get; private set; }
        public int lineNum { get; set; }
        public int charIndex { get; set; }

        public FileHandler()
        {
            lines = new List<string>();
            currentLine = "";
            lineNum = 1;
            charIndex = 0;
        }

        public void ReadLines(string filePath)
        {
            lines = File.ReadAllLines(filePath).ToList();
            SetCurrentLine();
        }

        public void GoToNextLine()
        {
            lineNum++;
            charIndex = 0;
            SetCurrentLine();
        }

        public void SetCurrentLine()
        {
            if (!IsEndOfFile())
            {
                currentLine = lines[lineNum - 1];
            }
        }

        public bool IsEndOfFile()
        {
            return lineNum > lines.Count();
        }
    }
}
