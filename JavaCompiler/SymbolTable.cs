using System;
using System.Collections.Generic;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public enum VarType { intType, booleanType, floatType }
    public enum ConstType { intType, floatType }
    public enum ReturnType { intType, booleanType, floatType, voidType }
    public enum PassingModes { passByValue, passByRef }
    public enum EntryType { varEntry, constEntry, methodEntry, classEntry }

    public class SymbolTable
    {
        public List<ITableEntry> symbolTable { get; set; }

        public SymbolTable()
        {
            symbolTable = new List<ITableEntry>();

            Variable entry = new Variable();
            entry.offset = 4;
            entry.size = 10;
            entry.lexeme = "test";
            symbolTable.Add(entry);

            if(symbolTable[0].typeOfEntry == EntryType.varEntry)
            {
                Variable testingVar = (Variable)symbolTable[0];
                Console.WriteLine(testingVar.offset);
            }
            //else if ...
        }

        public void Insert(string lexeme, Tokens token, int depth)
        {
            throw new NotImplementedException();
        }

        public ITableEntry Lookup(string lexeme)
        {
            throw new NotImplementedException();
        }
        
        public void DeleteDepth(int depth)
        {
            throw new NotImplementedException();
        }

        public void Display(int depth)
        {
            throw new NotImplementedException();
        }

        private int Hash(string lexeme)
        {
            throw new NotImplementedException();
        }
    }
}