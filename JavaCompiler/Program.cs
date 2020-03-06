using System;
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

            TableEntry entry2 = new TableEntry("testing", Tokens.IdT, 1);
            symbolTable.Upsert(entry2);
            Variable variable2 = symbolTable.Lookup("testing") as TableEntry;
            variable2.typeOfEntry = EntryType.varEntry;
            variable2.varType = VarType.floatType;
            variable2.offset = 6;
            variable2.size = 10;
            symbolTable.Upsert(variable2);
            symbolTable.Display(1);

            TableEntry entry3 = new TableEntry("testing", Tokens.IdT, 2);
            symbolTable.Upsert(entry3);
            Method method = symbolTable.Lookup("testing") as TableEntry;
            method.typeOfEntry = EntryType.methodEntry;
            method.numOfParameters = 1;
            method.parameterPassingModes.Add(PassingModes.passByValue);
            method.returnType = VarType.voidType;
            symbolTable.Upsert(method);
            symbolTable.Display(2);

            TableEntry entry4 = new TableEntry("another test", Tokens.IdT, 0);
            symbolTable.Upsert(entry4);
            Constant<int> constant = symbolTable.Lookup("another test") as TableEntry;
            constant.typeOfEntry = EntryType.constEntry;
            constant.value = 5;
            constant.offset = 12;
            symbolTable.Upsert(constant);
            symbolTable.Display(0);

            TableEntry entry5 = new TableEntry("testing class", Tokens.IdT, 0);
            symbolTable.Upsert(entry5);
            Class classEntry = symbolTable.Lookup("testing class") as TableEntry;
            classEntry.typeOfEntry = EntryType.classEntry;
            classEntry.sizeOfLocalVars = 5;
            classEntry.methodNames.Add("func1");
            symbolTable.Upsert(classEntry);
            symbolTable.Display(0);

            ITableEntry entry6 = symbolTable.Lookup("testing");
            Method method2 = null;

            if (entry6.typeOfEntry == EntryType.methodEntry)
            {
                method2 = entry6 as Method;
            }

            Console.WriteLine("Testing method field: expected value = 1, returned " + method2.numOfParameters);
            Console.WriteLine("Testing list size: expected value = 3, returned " + symbolTable.symbolTable[30].Count);
            Console.WriteLine("");

            symbolTable.Display(1);
            symbolTable.DeleteDepth(1);
            symbolTable.Display(1);

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