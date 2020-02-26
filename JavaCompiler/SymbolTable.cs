using System;
using System.Collections.Generic;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public enum VarType { intType, booleanType, voidType }
    public enum ConstType { intType, realType }
    public enum PassingModes { passByValue, passByRef }
    public enum EntryType { varEntry, constEntry, methodEntry, classEntry }

    public class Variable
    {
        public VarType varType { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
    }

    public class Constant
    {
        public ConstType constType { get; set; }
        public int offset { get; set; }
        public int value { get; set; }
        public float valueR { get; set; }
    }

    public class Class
    {
        public int sizeOfLocalVars { get; set; }
        public List<string> methodNames { get; set; }
        public List<string> varNames { get; set; }
    }

    public class Method
    {
        public List<VarType> parameterTypes { get; set; }
        public List<PassingModes> parameterPassingModes { get; set; }
        public int sizeOfLocalVars { get; set; }
        public int numOfParameters { get; set; }
    }

    public class TableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public dynamic entryData { get; set; }

        public TableEntry(EntryType type)
        {
            typeOfEntry = type;

            if (typeOfEntry == EntryType.varEntry) { entryData = new Variable(); }
            else if (typeOfEntry == EntryType.constEntry) { entryData = new Constant(); }
            else if (typeOfEntry == EntryType.classEntry) { entryData = new Class(); }
            else if (typeOfEntry == EntryType.methodEntry) { entryData = new Method(); }
        }
    }

    public class SymbolTable
    {
        public List<TableEntry> symbolTable { get; set; }

        public SymbolTable()
        {
            symbolTable = new List<TableEntry>();
        }

        public void Insert(string lexeme, Tokens token, int depth)
        {
            throw new NotImplementedException();
        }

        public TableEntry Lookup(string lexeme)
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