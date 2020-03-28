using System;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public static class ErrorHandler
    {
        /// <summary>
        /// Handles errors coming from Match(desired).
        /// </summary>
        public static void LogError(Tokens desired)
        {
            Console.WriteLine($"error - line {JavaFile.lineNum} - expected {GetExpectedLexeme(desired)}, found \"{Lexeme}\"");
            Environment.Exit(102);
        }

        /// <summary>
        /// Handles special case errors.
        /// </summary>
        public static void LogError(string errorMsg)
        {
            Console.WriteLine($"error - line {JavaFile.lineNum} - " + errorMsg);
            Environment.Exit(102);
        }

        /// <summary>
        /// Converts the expected token to its lexeme equivalent
        /// </summary>
        public static string GetExpectedLexeme(Tokens desired)
        {
            string expected = "";

            if ((int)desired < KeyWords.Count) { expected = "\"" + KeyWords[(int)desired] + "\""; }
            else if (OperatorTokens.Contains(desired)) { expected = "an operator"; }
            else if (desired == Tokens.IdT)     { expected = "an identifier"; }
            else if (desired == Tokens.NumT)    { expected = "a number"; }
            else if (desired == Tokens.LParenT) { expected = "\"(\""; }
            else if (desired == Tokens.RParenT) { expected = "\")\""; }
            else if (desired == Tokens.LBrackT) { expected = "\"[\""; }
            else if (desired == Tokens.RBrackT) { expected = "\"]\""; }
            else if (desired == Tokens.LBraceT) { expected = "\"{\""; }
            else if (desired == Tokens.RBraceT) { expected = "\"}\""; }
            else if (desired == Tokens.CommaT)  { expected = "\",\""; }
            else if (desired == Tokens.SemiT)   { expected = "\";\""; }
            else if (desired == Tokens.PeriodT) { expected = "\".\""; }

            return expected;
        }
    }
}