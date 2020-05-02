using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JavaCompiler
{
    public static class AssemblyFile
    {
        public static string mainName { get; set; }
        public static int literalNum { get; set; }
        private static List<Literal> literals { get; set; } = new List<Literal>();
        private static List<string> lines { get; set; } = new List<string>();

        public static void AddLiteral(string literal)
        {
            literals.Add(new Literal { label = $"S{literalNum}", literal = literal });
            literalNum++;
        }

        public static void AddLine(string line)
        {
            lines.Add(line);
        }

        public static void AddLiteralsToASMFile()
        {
            foreach (Literal literal in literals)
            {
                lines.Add(string.Format("{0, -4} DB    {1, 0}, \"$\"", literal.label, literal.literal));
            }
        }

        public static void CreateASMFile()
        {
            string asmFileName = JavaFile.fileName.Split(".")[0] + ".asm";

            using (StreamWriter sw = new StreamWriter(asmFileName))
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                    sw.WriteLine(line);
                }
            }
        }
    }
}
