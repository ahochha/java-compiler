using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Scanner : Resources
    {
        public List<Token> tokens { get; set; }
        public FileHandler javaFile { get; set; }

        public Scanner(FileHandler file)
        {
            javaFile = file;
            tokens = new List<Token>();
            javaFile.GetNextChar();

            //Resources
            Token = Symbol.UnknownT;
            Lexeme = "";
            Literal = "";
        }

        public void GetNextToken()
        {
            if (!javaFile.program.EndOfStream)
            {
                javaFile.SkipWhitespace();
                ProcessToken();
            }
            else
            {
                Token = Symbol.EofT;
            }
        }

        public void ProcessToken()
        {
            Regex letter = new Regex(@"[a-zA-Z]");
            Regex digit = new Regex(@"\d");
            Regex comparison = new Regex(@"<|>|!|=|&|\|");
            Regex lookAheadChar = new Regex(@"=|&|\|");
            Regex specialChar = new Regex(@"\(|\)|\[|\]|\{|\}|,|;|\.|\+|-|\*|/|=|<|>");
            string literal = "\"";

            ProcessComment();
            Lexeme = CurrentChar.ToString();
            javaFile.GetNextChar();

            if (letter.IsMatch(Lexeme))
            {
                ProcessWordToken();
            }
            else if (digit.IsMatch(Lexeme))
            {
                ProcessNumToken();
            }
            else if (comparison.IsMatch(Lexeme) && lookAheadChar.IsMatch(javaFile.PeekNextChar().ToString()))
            {
                ProcessDoubleToken();
            }
            else if (specialChar.IsMatch(Lexeme))
            {
                ProcessSingleToken();
            }
            else if (Lexeme == literal)
            {
                ProcessLiteral();
            }
            else
            {
                tokens.Add(new Token(Symbol.UnknownT, Lexeme));
            }
        }

        public void ProcessWordToken()
        {
            Regex word = new Regex(@"\w+");
            Regex print = new Regex(@"\.|[a-z]");

            LoadLexeme(word);

            if (Lexeme == "System")
            {
                LoadLexeme(print);
                Token = Symbol.PrintT;
            }
            else if (word.IsMatch(Lexeme) && Lexeme.Length <= 31)
            {
                Token = (KeyWords.Contains(Lexeme)) ? (Symbol)KeyWords.FindIndex(t => t == Lexeme) : Symbol.IdT;
            }
            else
            {
                Token = Symbol.UnknownT;
            }

            tokens.Add(new Token(Token, Lexeme));
        }

        public void ProcessNumToken()
        {
            Regex digit = new Regex(@"\w|\.");
            Regex number = new Regex(@"^(\d*\.)?\d+$");

            LoadLexeme(digit);
            Token = (number.IsMatch(Lexeme)) ? Symbol.NumT : Symbol.UnknownT;
            tokens.Add(new Token(Token, Lexeme));
        }

        public void ProcessDoubleToken()
        {
            Lexeme += CurrentChar;

            if (CurrentChar == '=')
            {
                Token = Symbol.RelOpT;
            }
            else if (CurrentChar == '&')
            {
                Token = Symbol.MulOpT;
            }
            else if (CurrentChar == '|')
            {
                Token = Symbol.AddOpT;
            }
            else
            {
                Token = Symbol.UnknownT;
            }

            tokens.Add(new Token(Token, Lexeme));
        }

        public void ProcessSingleToken()
        {
            if (Lexeme == "(")
            {
                Token = Symbol.LParenT;
            }
            else if (Lexeme == ")")
            {
                Token = Symbol.RParenT;
            }
            else if (Lexeme == "[")
            {
                Token = Symbol.LBrackT;
            }
            else if (Lexeme == "]")
            {
                Token = Symbol.RBrackT;
            }
            else if (Lexeme == "{")
            {
                Token = Symbol.LBraceT;
            }
            else if (Lexeme == "}")
            {
                Token = Symbol.RBraceT;
            }
            else if (Lexeme == ",")
            {
                Token = Symbol.CommaT;
            }
            else if (Lexeme == ";")
            {
                Token = Symbol.SemiT;
            }
            else if (Lexeme == ".")
            {
                Token = Symbol.PeriodT;
            }
            else if (Lexeme == "=")
            {
                Token = Symbol.AssignOpT;
            }
            else if (Lexeme == "+" || Lexeme == "-")
            {
                Token = Symbol.AddOpT;
            }
            else if (Lexeme == "*" || Lexeme == "/")
            {
                Token = Symbol.MulOpT;
            }
            else if (Lexeme == "<" || Lexeme == ">")
            {
                Token = Symbol.RelOpT;
            }
            else
            {
                Token = Symbol.UnknownT;
            }

            tokens.Add(new Token(Token, Lexeme));
        }

        public void ProcessLiteral()
        {
            tokens.Add(new Token(Symbol.QuoteT, Lexeme));
            Token = Symbol.LiteralT;

            while (CurrentChar != '\"' && !javaFile.program.EndOfStream)
            {
                Literal += CurrentChar;
                javaFile.GetNextChar();

                if (javaFile.PeekNextChar() == '\n')
                {
                    Token = Symbol.UnknownT;
                    break;
                }
            }

            tokens.Add(new Token(Token, Literal));

            if (CurrentChar == '\"')
            {
                Lexeme = CurrentChar.ToString();
                javaFile.GetNextChar();
                tokens.Add(new Token(Symbol.QuoteT, Lexeme));
            }
        }

        public void ProcessComment()
        {
            if (javaFile.PeekNextChar() == '/')
            {
                SkipComment(oneLineCommentEndRegex);
            }
            else if (javaFile.PeekNextChar() == '*')
            {
                SkipComment(multiLineCommentEndRegex);
                javaFile.GetNextChar();
                javaFile.SkipWhitespace();
            }
        }

        public void SkipComment(Regex commentEndRegex)
        {
            javaFile.GetNextChar();

            while (!commentEndRegex.IsMatch(CurrentChar.ToString() + javaFile.PeekNextChar()))
            {
                javaFile.GetNextChar();
            }

            javaFile.GetNextChar();
        }

        public void LoadLexeme(Regex lexemeRegex)
        {
            while (lexemeRegex.IsMatch(CurrentChar.ToString()))
            {
                Lexeme += CurrentChar;
                javaFile.GetNextChar();
            }
        }

    public void Print()
        {
            foreach(Token token in tokens)
            {
                Console.WriteLine(string.Format("Token: {0, -10}Attribute: {1, -10}", token.token, token.attribute));
            }
        }
    }
}
