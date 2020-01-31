using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Resources
    {
        public static List<string> KeyWords = new List<string>()
        {
            "class", "public", "static", "void", "main", "String",
            "extends", "return", "int", "boolean", "if", "else",
            "while", "System.out.println", "length", "true", "false",
            "this", "new"
        };

        public enum Symbol
        {
            ClassT, PublicT, StaticT, VoidT, MainT, StringT,
            ExtendsT, ReturnT, IntT, BooleanT, IfT, ElseT,
            WhileT, PrintT, LengthT, TrueT, FalseT, ThisT, NewT, 
            LParenT, RParenT, LBrackT, RBrackT, LBraceT, RBraceT, 
            CommaT, SemiT, PeriodT, IdT, NumT, LiteralT, QuoteT, 
            AssignOpT, AddOpT, MulOpT, RelOpT, EofT, UnknownT
        }

        public static Regex oneLineCommentEndRegex = new Regex(@"\n[^\r|\n]");
        public static Regex multiLineCommentEndRegex = new Regex(@"\*/");

        public static Symbol Token { get; set; }
        public static string Lexeme { get; set; }
        public static string Literal { get; set; }
        public static char CurrentChar { get; set; }
    }
}
