using System;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                JavaFile.ReadLines(args[0]);

                Parser parser = new Parser();
                Console.WriteLine("TAC File:");
                Console.WriteLine("---------");
                parser.Prog();

                if (Token == Tokens.EofT)
                {
                    Console.WriteLine("");
                    Console.WriteLine("successful compilation!");
                    Console.WriteLine("");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine($"error - line {JavaFile.lineNum} - unused tokens, please check for correct Java syntax");
                }

                JavaFile.CloseReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error - the compiler encountered an unknown error, please try compiling again");
                Console.WriteLine("note - be sure you have the file being input into the program as an application argument");
                Environment.Exit(100);
            }

            Console.WriteLine("Assembly File:");
            Console.WriteLine("--------------");
            AssemblyGenerator assemblyGenerator = new AssemblyGenerator();
            TACFile.ReadLinesFromFile();
            assemblyGenerator.GenerateASMFile();
        }
    }
}