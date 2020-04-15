using System;
using System.Collections.Generic;
using System.Linq;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public enum VarType { intType, booleanType, floatType, voidType }
    public enum PassingModes { passByValue, passByRef }
    public enum EntryType { tableEntry, varEntry, constEntry, methodEntry, classEntry }

    public class SymbolTable
    {
        public const int tableSize = 211;
        public List<ITableEntry>[] symbolTable { get; set; }

        public SymbolTable()
        {
            symbolTable = new List<ITableEntry>[tableSize];

            for (int i = 0; i < tableSize; i++)
            {
                symbolTable[i] = new List<ITableEntry>();
            }
        }

        /// <summary>
        /// Updates/Inserts into the symbol table. Updates the TableEntry to either Variable, Constant,
        /// Method, or Class if the depth matches the global depth at the found hash. Otherwise, the entry of
        /// type TableEntry is inserted to the front of the list at the calculated hash.
        /// </summary>
        public void Upsert(ITableEntry entry)
        {
            uint hash = Hash(entry.lexeme);

            if (entry.typeOfEntry != EntryType.tableEntry)
            {
                List<ITableEntry> entriesAtHash = symbolTable[hash];
                int index = entriesAtHash.FindIndex(tableEntry => tableEntry.lexeme == entry.lexeme && tableEntry.depth == entry.depth);
                symbolTable[hash][index] = entry;
            }
            else
            {
                symbolTable[hash].Insert(0, entry);
            }
        }

        /// <summary>
        /// Inserts basic table entry into the symbol table.
        /// </summary>
        public void InsertEntry(TableEntry entry)
        {
            CheckForDuplicates();
            Upsert(entry);
        }

        /// <summary>
        /// Creates a new table entry using the global Lexeme, Token, and Depth variables.
        /// </summary>
        public TableEntry CreateTableEntry()
        {
            TableEntry entry = new TableEntry(Lexeme, Token, Depth);
            InsertEntry(entry);

            return entry;
        }

        /// <summary>
        /// Converts the entry to a variable.
        /// </summary>
        public void ConvertEntryToVariable(ITableEntry entry)
        {
            Variable varEntry = entry as TableEntry;

            varEntry.typeOfEntry = EntryType.varEntry;
            varEntry.offset = Offset;
            varEntry.bpOffsetVarName = BpOffsetName;
            varEntry.varType = TypeVar;
            varEntry.size = Size;

            Upsert(varEntry);
        }

        /// <summary>
        /// Converts the entry to a class.
        /// </summary>
        public void ConvertEntryToClass(ITableEntry entry)
        {
            Class classEntry = entry as TableEntry;

            classEntry.typeOfEntry = EntryType.classEntry;
            classEntry.sizeOfLocalVars = LocalVarsSize;
            classEntry.varNames = VarNames;
            classEntry.methodNames = MethodNames;

            Upsert(classEntry);
        }

        /// <summary>
        /// Converts the entry to a method.
        /// </summary>
        public void ConvertEntryToMethod(ITableEntry entry)
        {
            Method methodEntry = entry as TableEntry;

            methodEntry.typeOfEntry = EntryType.methodEntry;
            methodEntry.returnType = TypeReturn;
            methodEntry.sizeOfLocalVars = LocalVarsSize;
            methodEntry.numOfParameters = ParameterNum;
            methodEntry.sizeOfParameterVars = ParameterVarsSize;
            methodEntry.parameterTypes = ParameterTypes;

            Upsert(methodEntry);
        }

        /// <summary>
        /// Converts the entry to a constant.
        /// </summary>
        public void ConvertEntryToConstant(ITableEntry entry)
        {
            GetConstantValue();

            if (TypeConst == VarType.floatType)
            {
                ConvertConstantBasedOnType<float>(entry);
            }
            else
            {
                ConvertConstantBasedOnType<int>(entry);
            }
        }

        /// <summary>
        /// Gets the constant value the variable is assigned to.
        /// </summary>
        public void GetConstantValue()
        {
            int value;
            float valueR;

            if (int.TryParse(Lexeme, out value))
            {
                Value = value;
            }
            else if (float.TryParse(Lexeme, out valueR))
            {
                ValueR = valueR;
            }
            else
            {
                ErrorHandler.LogError($"{Lexeme} is not a number.");
            }
        }

        /// <summary>
        /// Converts the entry to a constant of type int or float.
        /// </summary>
        public void ConvertConstantBasedOnType<ValueT>(ITableEntry entry)
        {
            Constant<ValueT> constEntry = entry as TableEntry;
            dynamic value;

            constEntry.typeOfEntry = EntryType.constEntry;
            constEntry.constType = TypeConst;
            constEntry.offset = Offset;

            if (TypeConst == VarType.floatType)
            {
                value = ValueR;
                constEntry.value = value;
            }
            else
            {
                value = Value;
                constEntry.value = value;
            }

            Upsert(constEntry);
        }

        /// <summary>
        /// Returns the table entry at the nearest depth that matches the given lexeme.
        /// </summary>
        public ITableEntry Lookup(string lexeme)
        {
            return symbolTable[Hash(lexeme)].FirstOrDefault(entry => entry.lexeme == lexeme);
        }

        /// <summary>
        /// Checks symbol table to verify that the given identifier is not a duplicate.
        /// </summary>
        public void CheckForDuplicates()
        {
            ITableEntry entry = Lookup(Lexeme);

            if (entry != null && entry.depth == Depth)
            {
                ErrorHandler.LogError($"duplicate identifier \"{Lexeme}\" found");
            }
        }

        /// <summary>
        /// Deletes all entries at the given depth.
        /// </summary>
        public void DeleteDepth(int depth)
        {
            foreach (List<ITableEntry> entries in symbolTable)
            {
                if (entries.Count > 0)
                {
                    entries.RemoveAll(entry => entry.depth == depth);
                }
            }
        }

        /// <summary>
        /// Returns the hashed value of the given lexeme.
        /// </summary>
        private uint Hash(string lexeme)
        {
            uint hash = 0, g;

            foreach (char c in lexeme)
            {
                hash = (hash << 4) + (byte)c;

                if ((g = hash & 0xf0000000) != 0)
                {
                    hash ^= (g >> 24);
                    hash ^= g;
                }
            }

            return hash % tableSize;
        }

        /// <summary>
        /// Displays the lexemes stored in the table at the given depth.
        /// </summary>
        public void Display(int depth)
        {
            Console.WriteLine($"Lexemes entered at depth {depth}:");
            Console.WriteLine("");
            Console.WriteLine(string.Format("Lexeme            Type of Entry"));
            Console.WriteLine("-------------------------------");

            foreach (List<ITableEntry> entries in symbolTable)
            {
                foreach (ITableEntry entry in entries)
                {
                    if (entry.depth == depth)
                    {
                        Console.WriteLine(string.Format("{0, -18}{1, -10}", entry.lexeme, entry.typeOfEntry));
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");
        }
    }
}