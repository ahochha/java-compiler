using System;
using System.Collections.Generic;
using System.Linq;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public enum VarType { intType, booleanType, floatType, voidType }
    public enum ConstType { intType, floatType }
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
            ITableEntry existingEntry = null;

            if (symbolTable[hash].Count > 0 && symbolTable[hash][0].depth == Depth)
            {
                existingEntry = symbolTable[hash][0];
            }

            if (existingEntry != null)
            {
                symbolTable[hash][0] = entry;
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
        /// Converts the entry to a variable.
        /// </summary>
        public void ConvertEntryToVariable(ITableEntry entry)
        {
            Variable varEntry = entry as Variable;

            varEntry.typeOfEntry = EntryType.varEntry;
            varEntry.offset = Offset;
            varEntry.varType = TypeVar;
            varEntry.size = Size;

            Upsert(varEntry);
        }

        /// <summary>
        /// Converts the entry to a constant.
        /// </summary>
        public void ConvertEntryToConstant(ITableEntry entry)
        {
            if (Lexeme.Contains("."))
            {
                Constant<float> constEntry = entry as Constant<float>;

                constEntry.typeOfEntry = EntryType.constEntry;
                constEntry.offset = Offset;
                constEntry.constType = ConstType.floatType;
                constEntry.value = ValueR;

                Upsert(constEntry);
            }
            else
            {
                Constant<int> constEntry = entry as Constant<int>;

                constEntry.typeOfEntry = EntryType.constEntry;
                constEntry.offset = Offset;
                constEntry.constType = ConstType.intType;
                constEntry.value = Value;

                Upsert(constEntry);
            }
        }

        /// <summary>
        /// Converts the entry to a class.
        /// </summary>
        public void ConvertEntryToClass(ITableEntry entry)
        {
            Class classEntry = entry as Class;

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
            Method methodEntry = entry as Method;

            methodEntry.typeOfEntry = EntryType.methodEntry;
            methodEntry.sizeOfLocalVars = LocalVarsSize;
            methodEntry.numOfParameters = ParameterNum;
            methodEntry.parameterPassingModes = ParameterPassingModes;
            methodEntry.parameterTypes = ParameterTypes;

            Upsert(methodEntry);
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

            if (entry != null && entry.typeOfEntry != EntryType.tableEntry && entry.depth == Depth)
            {
                ErrorHandler.LogError($"duplicate identifier {Lexeme} found");
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

            foreach (List<ITableEntry> entries in symbolTable)
            {
                foreach (ITableEntry entry in entries)
                {
                    if (entry.depth == depth)
                    {
                        Console.WriteLine(entry.lexeme + " " + entry.typeOfEntry);
                    }
                }
            }

            Console.WriteLine("");
        }
    }
}