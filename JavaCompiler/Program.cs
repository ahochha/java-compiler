using System;
using System.Collections.Generic;
using static JavaCompiler.Resources;

//Name: AUSTIN HOCHHALTER
//Class: CSC 446
//Assignment: 4
//Due Date: 3/6/2020
//Professor: HAMER

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolTable symbolTable = new SymbolTable();

            TableEntry entry = new TableEntry("testing", Tokens.IdT, 0);
            symbolTable.Upsert(entry);
            Variable variable = symbolTable.Lookup("testing") as TableEntry;
            variable.typeOfEntry = EntryType.varEntry;
            variable.varType = VarType.intType;
            variable.offset = 4;
            variable.size = 6;
            symbolTable.Upsert(variable);
            symbolTable.Display(0);

            //try
            //{
            //    JavaFile.ReadLines($@"{Environment.CurrentDirectory}\\" + args[0]);

            //    Parser parser = new Parser();
            //    parser.Prog();

            //    if (Token == Tokens.EofT)
            //    {
            //        Console.WriteLine("successful compilation!");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"error - line {JavaFile.lineNum} - unused tokens, please check for correct Java syntax");
            //    }

            //    JavaFile.CloseReader();
            //}
            //catch
            //{
            //    Console.WriteLine("error - the compiler encountered an unknown error, please try compiling again");
            //    Console.WriteLine("note - be sure you have the file being input into the program as an application argument");
            //    Environment.Exit(100);
            //}
        }
    }
}