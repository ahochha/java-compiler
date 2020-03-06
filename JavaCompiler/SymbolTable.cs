using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Method, or Class if the type of the found lexeme is TableEntry. Otherwise, the entry of
        /// type TableEntry is inserted to the front of the list at the calculated hash.
        /// </summary>
        public void Upsert(ITableEntry entry)
        {
            uint hash = Hash(entry.lexeme);
            ITableEntry existingEntry = null;

            if (symbolTable[hash].Count > 0 && symbolTable[hash][0].typeOfEntry == EntryType.tableEntry)
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
        /// Returns the table entry at the nearest depth that matches the given lexeme.
        /// </summary>
        public ITableEntry Lookup(string lexeme)
        {
            uint hash = Hash(lexeme);
            ITableEntry existingEntry = symbolTable[hash].FirstOrDefault(entry => entry.lexeme == lexeme);

            if (existingEntry == null)
            {
                ErrorHandler.LogError($"identifier \"{lexeme}\" doesn't exist");
            }

            return existingEntry;
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