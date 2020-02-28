using System.Collections.Generic;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public interface ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
    }

    public class Variable : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public VarType varType { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
    }

    public class Constant : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public ConstType constType { get; set; }
        public int offset { get; set; }
        public int value { get; set; }
        public float valueR { get; set; }
    }

    public class Class : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public int sizeOfLocalVars { get; set; }
        public List<string> methodNames { get; set; }
        public List<string> varNames { get; set; }
    }

    public class Method : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public List<VarType> parameterTypes { get; set; }
        public List<PassingModes> parameterPassingModes { get; set; }
        public int sizeOfLocalVars { get; set; }
        public int numOfParameters { get; set; }
        public ReturnType returnType { get; set; }
    }
}