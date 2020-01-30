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
            GetNextChar();

            //Resources
            Token = Symbol.UnknownT;
            Lexeme = "";
            Literal = "";
        }

        public void GetNextChar()
        {
            if (javaFile.charIndex < javaFile.currentLine.Length)
            {
                CurrentChar = javaFile.currentLine[javaFile.charIndex];
                javaFile.charIndex++;
            }
            else
            {
                javaFile.GoToNextLine();

                while (javaFile.currentLine.Length == 0)
                {
                    javaFile.GoToNextLine();
                }

                CurrentChar = javaFile.currentLine[javaFile.charIndex];
                javaFile.charIndex++;
            }
        }

        public void GetNextToken()
        {
            if (!javaFile.IsEndOfFile())
            {
                while (CurrentChar == ' ')
                {
                    GetNextChar();
                }

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
            Regex comparison = new Regex(@"<|>|!|=");
            Regex special = new Regex(@"\(|\)|\[|\]|\{|\}|,|;|\.|\+|-|\*|/");
            Regex literal = new Regex("\"");
            Regex comment = new Regex(@"/");

            Lexeme = CurrentChar.ToString();
            GetNextChar();

            if (letter.IsMatch(Lexeme))
            {
                ProcessWordToken();
            }
            else if (digit.IsMatch(Lexeme))
            {
                ProcessNumToken();
            }
            else if (comparison.IsMatch(Lexeme))
            {
                ProcessComparisonToken();
            }
            else if (special.IsMatch(Lexeme))
            {
                ProcessSingleToken();
            }
            else if (literal.IsMatch(Lexeme))
            {
                ProcessLiteral();
            }
            else if (comment.IsMatch(Lexeme))
            {
                ProcessComment();
            }
            else
            {
                //log error for invalid token
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
                tokens.Add(new Token(Symbol.PrintT, Lexeme));
            }
            else if (word.IsMatch(Lexeme) && Lexeme.Length <= 31)
            {
                Token = (KeyWords.Contains(Lexeme)) ? (Symbol)KeyWords.FindIndex(t => t == Lexeme) : Symbol.IdT;
                tokens.Add(new Token(Token, Lexeme));
            }
            else
            {
                //log error for invalid word token
            }
        }

        public void ProcessNumToken()
        {
            Regex digit = new Regex(@"\d|\.");
            Regex number = new Regex(@"(\d*\.)?\d+");

            LoadLexeme(digit);

            if (number.IsMatch(Lexeme))
            {
                tokens.Add(new Token(Symbol.NumT, Lexeme));
            }
            else
            {
                //log error for invalid number
            }
        }

        public void ProcessComparisonToken()
        {
            if (CurrentChar != ' ')
            {
                ProcessDoubleToken();
            }
            else
            {
                ProcessSingleToken();
            }
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
                //log error for invalid double token
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
                //log error for invalid special character token
            }

            tokens.Add(new Token(Token, Lexeme));
        }

        public void ProcessLiteral()
        {
            tokens.Add(new Token(Symbol.QuoteT, Lexeme));

            while (CurrentChar != '\"')
            {
                Literal += CurrentChar;
                GetNextChar();

                if (javaFile.currentLine.Length >= javaFile.charIndex)
                {
                    break;
                }
            }

            tokens.Add(new Token(Symbol.LiteralT, Literal));
            
            if(CurrentChar == '\"')
            {
                Lexeme = CurrentChar.ToString();
                tokens.Add(new Token(Symbol.QuoteT, Lexeme));
            }
            else
            {
                //log error for missing quotes
            }
            
        }

        public void ProcessComment()
        {
            Regex multiLineCommentEnd = new Regex(@"\*/");

            if (CurrentChar == '/')
            {
                javaFile.lineNum++;
                javaFile.charIndex = 0;
                javaFile.SetCurrentLine();
            }
            else if (CurrentChar == '*')
            {
                GetNextChar();

                while (multiLineCommentEnd.Match(Lexeme).Length == 0)
                {
                    Lexeme += CurrentChar;
                    GetNextChar();
                }
            }
            else
            {
                //log error for random /
            }
        }

        public void LoadLexeme(Regex regex)
        {
            while (regex.IsMatch(CurrentChar.ToString()))
            {
                Lexeme += CurrentChar;
                GetNextChar();
            }
        }
    }
}
