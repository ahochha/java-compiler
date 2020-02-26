using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public enum VarType { charType, intType, voidType }
    public enum EntryType { varEntry, constEntry, methodEntry, classEntry }

    class Variable
    {
        public VarType typeOfVariable { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
    }

    class Constant
    {
        public VarType typeOfConstant { get; set; }
        public int offset { get; set; }
        public int value { get; set; }
        public float valueR { get; set; }
    }

    class TableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
    }

    public class SymbolTable
    {
    }
}
