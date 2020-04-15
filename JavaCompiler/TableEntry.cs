using System.Collections.Generic;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    /// <summary>
    /// Interface used to define table entry types.
    /// </summary>
    public interface ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
    }

    /// <summary>
    /// Interface used to declare a constant as int or float.
    /// </summary>
    public interface IValue<ValueT>
    {
        public ValueT value { get; set; }
    }

    /// <summary>
    /// Implements ITableEntry. Only contains generic table entry fields.
    /// </summary>
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

    /// <summary>
    /// Implements ITableEntry. Also contains variable specific fields.
    /// </summary>
    public class Variable : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public VarType varType { get; set; }
        public int offset { get; set; }
        public string bpOffsetVarName { get; set; }
        public int size { get; set; }

        public Variable()
        {

        }

        public Variable(Variable var)
        {
            token = var.token;
            lexeme = var.lexeme;
            depth = var.depth;
            typeOfEntry = var.typeOfEntry;
            varType = var.varType;
            offset = var.offset;
            bpOffsetVarName = var.bpOffsetVarName;
            size = var.size;
        }

        public static implicit operator Variable(TableEntry entry)
        {
            return (entry != null) ? new Variable()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            } 
            : new Variable()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0
            };
        }
    }

    /// <summary>
    /// Implements ITableEntry and IValue. Also contains constant specific fields.
    /// </summary>
    public class Constant<ValueT> : ITableEntry, IValue<ValueT>
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public VarType constType { get; set; }
        public int offset { get; set; }
        public ValueT value { get; set; }

        public static implicit operator Constant<ValueT>(TableEntry entry)
        {
            return (entry != null) ? new Constant<ValueT>()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            }
            : new Constant<ValueT>()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0
            };
        }
    }

    /// <summary>
    /// Implements ITableEntry. Also contains class specific fields.
    /// </summary>
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
            } 
            : new Class()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0
            };
        }
    }

    /// <summary>
    /// Implements ITableEntry. Also contains method specific fields.
    /// </summary>
    public class Method : ITableEntry
    {
        public Tokens token { get; set; }
        public string lexeme { get; set; }
        public int depth { get; set; }
        public EntryType typeOfEntry { get; set; }
        public int sizeOfLocalVars { get; set; }
        public List<VarType> parameterTypes { get; set; } = new List<VarType>();
        public List<string> parameterNames { get; set; } = new List<string>();
        public int sizeOfParameterVars { get; set; }
        public int numOfParameters { get; set; }
        public VarType returnType { get; set; }

        public static implicit operator Method(TableEntry entry)
        {
            return (entry != null) ? new Method()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth
            }
            : new Method()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0
            };
        }
    }
}