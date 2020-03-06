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

    public interface IValue<ValueT>
    {
        public ValueT value { get; set; }
    }

    public class TableEntry : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }

        public TableEntry(string _lexeme, Tokens _token, int _depth)
        {
            lexeme = _lexeme;
            token = _token;
            depth = _depth;
        }
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

        public static implicit operator Variable(TableEntry entry)
        {
            return (entry != null) ? new Variable()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            } : new Variable();
        }
    }

    public class Constant<ValueT> : ITableEntry, IValue<ValueT>
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public ConstType constType { get; set; }
        public int offset { get; set; }
        public ValueT value { get; set; }

        public static implicit operator Constant<ValueT>(TableEntry entry)
        {
            return (entry != null) ? new Constant<ValueT>()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            } : new Constant<ValueT>();
        }
    }

    public class Class : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public int sizeOfLocalVars { get; set; }
        public List<string> methodNames { get; set; } = new List<string>();
        public List<string> varNames { get; set; } = new List<string>();

        public static implicit operator Class(TableEntry entry)
        {
            return (entry != null) ? new Class()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            } : new Class();
        }
    }

    public class Method : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public List<VarType> parameterTypes { get; set; } = new List<VarType>();
        public List<PassingModes> parameterPassingModes { get; set; } = new List<PassingModes>();
        public int sizeOfLocalVars { get; set; }
        public int numOfParameters { get; set; }
        public VarType returnType { get; set; }

        public static implicit operator Method(TableEntry entry)
        {
            return (entry != null) ? new Method()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            } : new Method();
        }
    }
}