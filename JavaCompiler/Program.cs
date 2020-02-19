﻿using System;
using static JavaCompiler.Resources;

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
                JavaFile.ReadLines($@"{Environment.CurrentDirectory}\\" + args[0]);

                Parser parser = new Parser();
                parser.Prog();

                if (Token == Tokens.EofT)
                {
                    Console.WriteLine("successful compilation!");
                }
                else
                {
                    Console.WriteLine("error - unused tokens, please check for correct Java syntax");
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