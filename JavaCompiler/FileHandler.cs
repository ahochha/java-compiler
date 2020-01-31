using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JavaCompiler
{
    public class FileHandler : Resources
    {
        public StreamReader program {get; set; }
        public int lineNum { get; set; }

        public FileHandler()
        {
            lineNum = 1;
        }

        public void ReadLines(string filePath)
        {
            program = File.OpenText(filePath);
        }

        public void GetNextChar()
        {
            CurrentChar = (char)program.Read();
            
            if (CurrentChar == '\n')
            {
                lineNum++;
            }
        }

        public char PeekNextChar()
        {
            return (char)program.Peek();
        }

        public void SkipWhitespace()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                GetNextChar();
            }
        }
    }
}
