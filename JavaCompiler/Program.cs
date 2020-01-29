using System;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            FileHandler javaFile = new FileHandler();
            javaFile.ReadLines($@"{Environment.CurrentDirectory}\\..\\..\\..\\" + args[0]);

            Scanner scanner = new Scanner(javaFile);

            while (Resources.Token != Resources.Symbol.EofT)
            {
                scanner.GetNextToken();
            }
        }
    }
}
