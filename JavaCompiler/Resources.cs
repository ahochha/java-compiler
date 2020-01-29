using System;
using System.Collections.Generic;
using System.Text;

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
            WhileT, PrintT, LengthT, TrueT, FalseT, ThisT, 
            NewT, LParenT, RParenT, LBrackT, RBrackT, LBraceT,
            RBraceT, CommaT, SemiT, AsterT, PeriodT, IdT,
            QuoteT, AddOpT, MulOpT, AssignOpT, EofT, UnknownT
        }

        public static Symbol Token { get; set; }
        public static string Lexeme { get; set; }
        public static string Literal { get; set; }
        public static char CurrentChar { get; set; }
        public static int Value { get; set; }
        public static double ValueR { get; set; }
    }
}
