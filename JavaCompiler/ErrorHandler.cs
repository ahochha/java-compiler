using System;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public static class ErrorHandler
    {
        public static void LogError(Symbol desired)
        {
            if (desired != Symbol.UnknownT)
            {
                Console.WriteLine($"ERROR - Line {JavaFile.lineNum} - Expected {GetExpectedLexeme(desired)}, found \"{Lexeme}\"");
            }
            else
            {
                Console.WriteLine($"ERROR - Line {JavaFile.lineNum} - " + SpecialErrorMsg);
            }

            Environment.Exit(102);
        }

        public static string GetExpectedLexeme(Symbol desired)
        {
            string expected = "";

            if ((int)desired < KeyWords.Count) { expected = "\"" + KeyWords[(int)desired] + "\""; }
            else if (OperatorTokens.Contains(desired)) { expected = "an operator"; }
            else if (desired == Symbol.IdT)     { expected = "an identifier"; }
            else if (desired == Symbol.LParenT) { expected = "\"(\""; }
            else if (desired == Symbol.RParenT) { expected = "\")\""; }
            else if (desired == Symbol.LBrackT) { expected = "\"[\""; }
            else if (desired == Symbol.RBrackT) { expected = "\"]\""; }
            else if (desired == Symbol.LBraceT) { expected = "\"{\""; }
            else if (desired == Symbol.RBraceT) { expected = "\"}\""; }
            else if (desired == Symbol.CommaT)  { expected = "\",\""; }
            else if (desired == Symbol.SemiT)   { expected = "\";\""; }
            else if (desired == Symbol.PeriodT) { expected = "\".\""; }

            return expected;
        }

        public static void LogDetailedError(string errorMsg)
        {
            SpecialErrorMsg = errorMsg;
            LogError(Symbol.UnknownT);
        }
    }
}
