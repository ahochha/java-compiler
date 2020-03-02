using System;
using System.Collections.Generic;

namespace JavaCompiler
{
    public enum VarType { intType, booleanType, floatType, voidType }
    public enum ConstType { intType, floatType }
    public enum PassingModes { passByValue, passByRef }
    public enum EntryType { varEntry, constEntry, methodEntry, classEntry }

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

        public void Upsert(ITableEntry entry)
        {
            uint hash = Hash(entry.lexeme);
            ITableEntry existingEntry = (symbolTable[hash].Count > 0) ? symbolTable[hash][0] : null;

            if (existingEntry != null)
            {
                symbolTable[hash][0] = entry;
            }
            else
            {
                symbolTable[hash].Insert(0, entry);
            }
        }

        public ITableEntry Lookup(string lexeme)
        {
            uint hash = Hash(lexeme);

            return (symbolTable[hash].Count > 0) ? symbolTable[Hash(lexeme)][0] : null;
        }
        
        public void DeleteDepth(int depth)
        {
            foreach (List<ITableEntry> entries in symbolTable)
            {
                if (entries.Count > depth)
                {
                    entries.RemoveAt(depth);
                }
            }
        }

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

        public void Display(int depth)
        {
            Console.WriteLine("Entry Lexemes:");

            foreach (List<ITableEntry> entries in symbolTable)
            {
                if (entries.Count > depth)
                {
                    Console.WriteLine(entries[depth].lexeme);
                }
            }
        }
    }
}