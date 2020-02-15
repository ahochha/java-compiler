using System;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public static class ErrorHandler
    {
        public static void LogError(int lineNum, Symbol desired)
        {
            Console.WriteLine($"ERROR - Line {lineNum} - Expected \"{GetExpectedLexeme(desired)}\", found \"{Lexeme}\"");
            Environment.Exit(102);
        }

        public static string GetExpectedLexeme(Symbol desired)
        {
            string expected = "";

            if ((int)desired < KeyWords.Count) { expected = KeyWords[(int)desired]; }
            else if (OperatorTokens.Contains(desired)) { expected = "an operator"; }
            else if (desired == Symbol.LParenT) { expected = "("; }
            else if (desired == Symbol.RParenT) { expected = ")"; }
            else if (desired == Symbol.LBrackT) { expected = "["; }
            else if (desired == Symbol.RBrackT) { expected = "]"; }
            else if (desired == Symbol.LBraceT) { expected = "{"; }
            else if (desired == Symbol.RBraceT) { expected = "}"; }
            else if (desired == Symbol.CommaT)  { expected = ","; }
            else if (desired == Symbol.SemiT)   { expected = ";"; }
            else if (desired == Symbol.PeriodT) { expected = "."; }

            return expected;
        }
    }
}
