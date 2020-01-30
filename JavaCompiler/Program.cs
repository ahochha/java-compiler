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

            scanner.tokens.Add(new Token(Resources.Token, "Done!"));

            foreach(Token token in scanner.tokens)
            {
                Console.WriteLine($"Token: {token.token}  Attribute: {token.attribute}");
            }
        }
    }
}
