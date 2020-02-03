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

        /// <summary>
        /// Reads the java program into a StreamReader.
        /// </summary>
        public void ReadLines(string filePath)
        {
            try
            {
                program = File.OpenText(filePath);
            }
            catch
            {
                Console.WriteLine("Unable to read the Java file.");
                Console.WriteLine("Be sure you have the file in the same directory as the exe.");
                Console.WriteLine("This should be the netcoreapp3.0 folder.");
            }
        }

        /// <summary>
        /// Reads the next character from the program.
        /// </summary>
        public void GetNextChar()
        {
            CurrentChar = (char)program.Read();
            
            if (CurrentChar == '\n')
            {
                lineNum++;
            }
        }

        /// <summary>
        /// Returns the next character in the program without
        /// moving the StreamReader index.
        /// </summary>
        public char PeekNextChar()
        {
            return (char)program.Peek();
        }

        /// <summary>
        /// Skips whitespace characters (\n, \t, \r, etc..)
        /// </summary>
        public void SkipWhitespace()
        {
            while (char.IsWhiteSpace(CurrentChar))
            {
                GetNextChar();
            }
        }
    }
}