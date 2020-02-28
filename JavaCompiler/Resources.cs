using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public static class Resources
    {
        // Java reserved words
        public static List<string> KeyWords = new List<string>()
        {
            "final", "class", "public", "static", "void", "main",
            "String", "extends", "return", "int", "boolean", "float", 
            "if", "else", "while", "System.out.println", "length", 
            "true", "false", "this", "new"
        };

        // Types of Tokens
        public enum Tokens
        {
            FinalT, ClassT, PublicT, StaticT, VoidT, MainT,
            StringT, ExtendsT, ReturnT, IntT, BooleanT, FloatT, IfT, 
            ElseT, WhileT, PrintT, LengthT, TrueT, FalseT, ThisT, 
            NewT, LParenT, RParenT, LBrackT, RBrackT, LBraceT, RBraceT,
            CommaT, SemiT, PeriodT, IdT, NumT, LiteralT, QuoteT,
            AssignOpT, AddOpT, MulOpT, RelOpT, EofT, UnknownT
        }

        // Data types supported
        public static List<Tokens> Types = new List<Tokens>
        {
            Tokens.IntT, Tokens.BooleanT, Tokens.VoidT
        };

        // Tokens that are operators
        public static List<Tokens> OperatorTokens = new List<Tokens>()
        {
            Tokens.AssignOpT, Tokens.RelOpT, Tokens.AddOpT, Tokens.MulOpT
        };

        // Regular expressions used in LexicalAnalyzer
        public static Regex comparisonRegex = new Regex(@"<|>|!|=|&|\|");
        public static Regex lookAheadCharRegex = new Regex(@"=|&|\|");
        public static Regex specialCharRegex = new Regex(@"\(|\)|\[|\]|\{|\}|,|;|\.|\+|-|\*|/|=|<|>");
        public static Regex commentStartRegex = new Regex(@"^/[/|\*]$");
        public static Regex wordRegex = new Regex(@"\w+");
        public static Regex printRegex = new Regex(@"\.|[a-z]");
        public static Regex decimalDigitRegex = new Regex(@"\d|\.");
        public static Regex numberRegex = new Regex(@"^(\d+\.)?\d+$");
        public static Regex oneLineCommentEndRegex = new Regex(@"\n[^\r|\n]");
        public static Regex multiLineCommentEndRegex = new Regex(@"\*/");

        public static Tokens Token { get; set; }
        public static string Lexeme { get; set; }
        public static string Literal { get; set; }
        public static char CurrentChar { get; set; }
        public static int Value { get; set; }
        public static double ValueR { get; set; }
    }
}