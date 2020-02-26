using System;
using System.IO;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public static class JavaFile
    {
        private static StreamReader program {get; set; }
        public static int lineNum { get; set; } = 1;

        /// <summary>
        /// Reads the java program into a StreamReader.
        /// </summary>
        public static void ReadLines(string filePath)
        {
            try
            {
                program = File.OpenText(filePath);
            }
            catch
            {
                Console.WriteLine("error - unable to read the missing Java file");
                Console.WriteLine("note - please place your test file in JavaCompiler > bin > Debug > netcoreapp3.0");
                Environment.Exit(101);
            }
        }

        /// <summary>
        /// Reads the next character from the program.
        /// </summary>
        public static void GetNextChar()
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
        public static char PeekNextChar()
        {
            return (char)program.Peek();
        }

        /// <summary>
        /// Skips whitespace characters (\n, \t, \r, etc..)
        /// </summary>
        public static void SkipWhitespace()
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
        public static bool EndOfFile()
        {
            return program.EndOfStream;
        }

        /// <summary>
        /// Closes the StreamReader.
        /// </summary>
        public static void CloseReader()
        {
            program.Close();
        }
    }
}