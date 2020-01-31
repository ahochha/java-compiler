using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JavaCompiler
{
    public class FileHandler
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
            Resources.CurrentChar = (char)program.Read();
            
            if (Resources.CurrentChar == '\n')
            {
                lineNum++;
                Resources.CurrentChar = (char)program.Read();
            }
        }

        public char PeekNextChar()
        {
            return (char)program.Peek();
        }
    }
}
