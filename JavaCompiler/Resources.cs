using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public static class Resources
    {
        #region Lexical Analyzer
        // Java reserved words
        public static List<string> KeyWords = new List<string>()
        {
            "final", "class", "public", "static", "void", "main",
            "String", "extends", "return", "int", "boolean", "float",
            "if", "else", "while", "System.out.println", "length",
            "true", "false", "this", "new", "write", "writeln", "read"
        };

        // Types of Tokens
        public enum Tokens
        {
            FinalT, ClassT, PublicT, StaticT, VoidT, MainT,
            StringT, ExtendsT, ReturnT, IntT, BooleanT, FloatT, IfT,
            ElseT, WhileT, PrintT, LengthT, TrueT, FalseT, ThisT,
            NewT, WriteT, WritelnT, ReadT, LParenT, RParenT, LBrackT, RBrackT, 
            LBraceT, RBraceT, CommaT, SemiT, PeriodT, IdT, NumT, LiteralT, 
            QuoteT, AssignOpT, NotOpT, AddOpT, MulOpT, RelOpT, EofT, UnknownT
        }

        // Data types supported
        public static List<Tokens> Types = new List<Tokens>
        {
            Tokens.IntT, Tokens.FloatT, Tokens.BooleanT, Tokens.VoidT
        };

        // Regular expressions used in LexicalAnalyzer
        public static Regex comparisonRegex = new Regex(@"<|>|!|=|&|\|");
        public static Regex lookAheadCharRegex = new Regex(@"=|&|\|");
        public static Regex specialCharRegex = new Regex(@"\(|\)|\[|\]|\{|\}|,|;|\.|\+|-|\*|/|=|!|<|>");
        public static Regex commentStartRegex = new Regex(@"^/[/|\*]$");
        public static Regex wordRegex = new Regex(@"\w+");
        public static Regex printRegex = new Regex(@"\.|[a-z]");
        public static Regex decimalDigitRegex = new Regex(@"\d|\.");
        public static Regex numberRegex = new Regex(@"^(\d+\.)?\d+$");
        public static Regex oneLineCommentEndRegex = new Regex(@"\n[^\r|\n]");
        public static Regex multiLineCommentEndRegex = new Regex(@"\*/");
        #endregion

        #region Parser

        // Tokens that are operators
        public static List<Tokens> OperatorTokens = new List<Tokens>()
        {
            Tokens.AssignOpT, Tokens.NotOpT, Tokens.RelOpT, Tokens.AddOpT, Tokens.MulOpT
        };

        // Tokens that are factors
        public static List<Tokens> FactorTokens = new List<Tokens>()
        {
            Tokens.IdT, Tokens.NumT, Tokens.LParenT, Tokens.NotOpT, Tokens.AddOpT,
            Tokens.TrueT, Tokens.FalseT
        };

        // General
        public static Tokens Token { get; set; }
        public static string Lexeme { get; set; }
        public static char CurrentChar { get; set; }
        public static int Depth { get; set; }
        public static int Offset { get; set; }
        public static string BpOffsetName { get; set; }
        public static int LocalVarsSize { get; set; }

        // Variable
        public static VarType TypeVar { get; set; }
        public static int Size { get; set; }

        // Constant
        public static VarType TypeConst { get; set; }
        public static int? Value { get; set; }
        public static float? ValueR { get; set; }

        // Class
        public static List<string> MethodNames { get; set; } = new List<string>();
        public static List<string> VarNames { get; set; } = new List<string>();

        // Method
        public static List<string> ParameterNames { get; set; } = new List<string>();
        public static List<VarType> ParameterTypes { get; set; } = new List<VarType>();
        public static int ParameterVarsSize { get; set; }
        public static int ParameterNum { get; set; }
        public static VarType TypeReturn { get; set; }

        /// <summary>
        /// Clears method entry globals after entering a new method.
        /// </summary>
        public static void ResetMethodGlobals()
        {
            Offset = 0;
            LocalVarsSize = 0;
            ParameterTypes = new List<VarType>();
            ParameterNames = new List<string>();
            ParameterVarsSize = 0;
            VarNames = new List<string>();
            ParameterNum = 0;
        }

        /// <summary>
        /// Clears class entry globals after entering a new class.
        /// </summary>
        public static void ResetClassGlobals()
        {
            Offset = 0;
            LocalVarsSize = 0;
            MethodNames = new List<string>();
            VarNames = new List<string>();
        }

        #endregion

        #region ASM file Generation
        public static List<string> TACKeywords = new List<string>()
        {
            "proc", "endp", "wrs", "wri", "wrln", "rdi", "call", "push"
        };

        public static Queue<Method> ASMProcs = new Queue<Method>();
        #endregion
    }
}