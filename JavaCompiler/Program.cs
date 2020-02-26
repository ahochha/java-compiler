using System;
using static JavaCompiler.Resources;

//Name: AUSTIN HOCHHALTER
//Class: CSC 446
//Assignment: 3
//Due Date: 2/21/2020
//Professor: HAMER

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                JavaFile.ReadLines($@"{Environment.CurrentDirectory}\\" + args[0]);

                Parser parser = new Parser();
                parser.Prog();

                if (Token == Tokens.EofT)
                {
                    Console.WriteLine("successful compilation!");
                }
                else
                {
                    Console.WriteLine($"error - line {JavaFile.lineNum} - unused tokens, please check for correct Java syntax");
                }

                JavaFile.CloseReader();
            }
            catch
            {
                Console.WriteLine("note - the compiler encountered an error, please try compiling again");
                Environment.Exit(100);
            }
        }
    }
}