using System;

//Name: AUSTIN HOCHHALTER
//Class: CSC 446
//Assignment: 1
//Due Date: 2/5/2020
//Professor: HAMER

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileHandler javaFile = new FileHandler();
                javaFile.ReadLines($@"{Environment.CurrentDirectory}\\" + args[0]);

                Parser parser = new Parser(javaFile);
                parser.Prog();

                if (Resources.Token == Resources.Symbol.EofT)
                {
                    Console.WriteLine("Successful compilation!");
                }
                else
                {
                    Console.WriteLine("ERROR - Unused tokens");
                }

                javaFile.CloseReader();
            }
            catch
            {
                Console.WriteLine("Please place your test file in JavaCompiler > bin > Debug > netcoreapp3.0");
                Console.WriteLine("Also be sure you have the file being input into the program as an application argument.");
            }
        }
    }
}