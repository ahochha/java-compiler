using System;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            FileHandler.ReadLines($@"{Environment.CurrentDirectory}\\..\\..\\..\\" + args[0]);
            Scanner scanner = new Scanner();

            while (Resources.Token != Resources.Symbol.EofT)
            {
                scanner.GetNextToken();
            }
        }
    }
}
