using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace JavaCompiler
{
    public class FileHandler : Resources
    {
        private StreamReader program {get; set; }
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
                Console.WriteLine("ERROR - Unable to read the missing Java file.");
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
            while (char.IsWhiteSpace(PeekNextChar()))
            {
                GetNextChar();
            }
        }

        /// <summary>
        /// Checks to see if the current character is at the
        /// end of the file.
        /// </summary>
        public bool EndOfFile()
        {
            return program.EndOfStream;
        }

        /// <summary>
        /// Closes the StreamReader.
        /// </summary>
        public void CloseReader()
        {
            program.Close();
        }
    }
}