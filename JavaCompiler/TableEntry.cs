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
        public string bpOffsetName { get; set; }
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
        public string bpOffsetName { get; set; }

        public TableEntry(string _lexeme, Tokens _token, int _depth)
        {
            lexeme = _lexeme;
            token = _token;
            depth = _depth;
            bpOffsetName = "";
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
        public string bpOffsetName { get; set; }
        public VarType varType { get; set; }
        public int offset { get; set; }
        public int size { get; set; }

        public Variable()
        {

        }

        public Variable(Variable entry)
        {
            token = entry.token;
            lexeme = entry.lexeme;
            depth = entry.depth;
            typeOfEntry = entry.typeOfEntry;
            bpOffsetName = entry.bpOffsetName;
            varType = entry.varType;
            offset = entry.offset;
            size = entry.size;
        }

        public static implicit operator Variable(TableEntry entry)
        {
            return (entry != null) ? new Variable()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth,
                bpOffsetName = entry.bpOffsetName
            } 
            : new Variable()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0,
                bpOffsetName = ""
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
        public string bpOffsetName { get; set; }
        public VarType constType { get; set; }
        public int offset { get; set; }
        public ValueT value { get; set; }

        public Constant()
        {
            
        }

        public Constant(Constant<ValueT> entry)
        {
            token = entry.token;
            lexeme = entry.lexeme;
            depth = entry.depth;
            typeOfEntry = entry.typeOfEntry;
            bpOffsetName = entry.bpOffsetName;
            constType = entry.constType;
            offset = entry.offset;
            value = entry.value;
        }

        public static implicit operator Constant<ValueT>(TableEntry entry)
        {
            return (entry != null) ? new Constant<ValueT>()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth,
                bpOffsetName = entry.bpOffsetName
            }
            : new Constant<ValueT>()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0,
                bpOffsetName = ""
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
        public string bpOffsetName { get; set; }
        public int sizeOfLocalVars { get; set; }
        public List<string> methodNames { get; set; } = new List<string>();
        public List<string> varNames { get; set; } = new List<string>();

        public Class()
        {

        }

        public Class(Class entry)
        {
            token = entry.token;
            lexeme = entry.lexeme;
            depth = entry.depth;
            typeOfEntry = entry.typeOfEntry;
            bpOffsetName = entry.bpOffsetName;
            sizeOfLocalVars = entry.sizeOfLocalVars;
            methodNames = entry.methodNames;
            varNames = entry.varNames;
        }

        public static implicit operator Class(TableEntry entry)
        {
            return (entry != null) ? new Class()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth,
                bpOffsetName = entry.bpOffsetName
            } 
            : new Class()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0,
                bpOffsetName = ""
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
        public string bpOffsetName { get; set; }
        public int sizeOfLocalVars { get; set; }
        public List<VarType> parameterTypes { get; set; } = new List<VarType>();
        public List<string> parameterNames { get; set; } = new List<string>();
        public int sizeOfParameterVars { get; set; }
        public int numOfParameters { get; set; }
        public VarType returnType { get; set; }

        public Method()
        {

        }

        public Method(Method entry)
        {
            token = entry.token;
            lexeme = entry.lexeme;
            depth = entry.depth;
            typeOfEntry = entry.typeOfEntry;
            bpOffsetName = entry.bpOffsetName;
            sizeOfLocalVars = entry.sizeOfLocalVars;
            parameterTypes = entry.parameterTypes;
            parameterNames = entry.parameterNames;
            sizeOfParameterVars = entry.sizeOfParameterVars;
            numOfParameters = entry.numOfParameters;
            returnType = entry.returnType;
        }

        public static implicit operator Method(TableEntry entry)
        {
            return (entry != null) ? new Method()
            {
                lexeme = entry.lexeme,
                token = entry.token,
                depth = entry.depth,
                bpOffsetName = entry.bpOffsetName
            }
            : new Method()
            {
                lexeme = "",
                token = Tokens.UnknownT,
                depth = 0,
                bpOffsetName = ""
            };
        }
    }
}