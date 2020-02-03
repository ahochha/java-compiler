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

            // Resources
            Token = Symbol.UnknownT;
            Lexeme = "";
            Literal = "";
        }

        /// <summary>
        /// Processes tokens until end of stream has been hit.
        /// </summary>
        public void GetNextToken()
        {
            javaFile.SkipWhitespace();
            ProcessComment();

            if (!javaFile.program.EndOfStream)
            {
                ProcessToken();
            }
            else
            {
                Token = Symbol.EofT;
            }
        }

        /// <summary>
        /// Reads the first token char into Lexeme, which is then
        /// used to determine which function to use for processing.
        /// </summary>
        public void ProcessToken()
        {
            Lexeme = CurrentChar.ToString();
            javaFile.GetNextChar();

            if (char.IsLetter(Lexeme[0]) || Lexeme == "_")
            {
                ProcessWordToken();
            }
            else if (char.IsDigit(Lexeme[0]) || Lexeme == "." && char.IsDigit(CurrentChar))
            {
                ProcessNumToken();
            }
            else if (comparisonRegex.IsMatch(Lexeme) && lookAheadCharRegex.IsMatch(CurrentChar.ToString()))
            {
                ProcessDoubleToken();
            }
            else if (specialCharRegex.IsMatch(Lexeme))
            {
                ProcessSingleToken();
            }
            else if (Lexeme == "\"")
            {
                ProcessLiteral();
            }
            else
            {
                tokens.Add(new Token(Symbol.UnknownT, Lexeme));
            }
        }

        /// <summary>
        /// Reads characters into Lexeme, then validates for correct 
        /// word token. Word token can be a reserved word (KeyWords list) 
        /// or an identifier (IdT). Adds token to list.
        /// </summary>
        public void ProcessWordToken()
        {
            LoadLexeme(letterDigitUnderscoreRegex);

            if (Lexeme == "System")
            {
                LoadLexeme(printRegex);
                Token = Symbol.PrintT;
            }
            else if (wordRegex.IsMatch(Lexeme) && Lexeme.Length <= 31)
            {
                Token = (KeyWords.Contains(Lexeme)) ? (Symbol)KeyWords.FindIndex(t => t == Lexeme) : Symbol.IdT;
            }
            else
            {
                Token = Symbol.UnknownT;
            }

            tokens.Add(new Token(Token, Lexeme));
        }

        /// <summary>
        /// Reads characters into Lexeme, then validates for correct 
        /// number token. Number token (NumT) can be an integer or double 
        /// value. Adds token to list.
        /// </summary>
        public void ProcessNumToken()
        {
            LoadLexeme(decimalDigitRegex);
            Token = (numberRegex.IsMatch(Lexeme)) ? Symbol.NumT : Symbol.UnknownT;
            tokens.Add(new Token(Token, Lexeme));
        }

        /// <summary>
        /// Reads next character into Lexeme, then validates correct 
        /// double token. Adds token to list.
        /// </summary>
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

            javaFile.GetNextChar();
            tokens.Add(new Token(Token, Lexeme));
        }

        /// <summary>
        /// Validates correct single token. Adds token to list.
        /// </summary>
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

        /// <summary>
        /// Reads a literal string until the ending quote is found. 
        /// Adds 3 tokens: 2 quote tokens, 1 literal token.
        /// </summary>
        /// <example>
        /// "literal" => QuoteT -> LiteralT -> QuoteT
        /// </example>
        public void ProcessLiteral()
        {
            tokens.Add(new Token(Symbol.QuoteT, Lexeme));
            Token = Symbol.LiteralT;
            Literal = CurrentChar.ToString();
            javaFile.GetNextChar();

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

        /// <summary>
        /// Determines type of comment, the comment is then skipped.
        /// </summary>
        public void ProcessComment()
        {
            if (javaFile.PeekNextChar() == '/')
            {
                SkipComment(oneLineCommentEndRegex);
                javaFile.SkipWhitespace();
            }
            else if (javaFile.PeekNextChar() == '*')
            {
                SkipComment(multiLineCommentEndRegex);
                javaFile.GetNextChar();
                javaFile.SkipWhitespace();
            }

            if (javaFile.PeekNextChar() == '/' || javaFile.PeekNextChar() == '*')
            {
                ProcessComment();
            }
        }

        /// <summary>
        /// Calls GetNextChar() to skip over comment characters.
        /// </summary>
        public void SkipComment(Regex commentEndRegex)
        {
            javaFile.GetNextChar();

            while (!commentEndRegex.IsMatch(CurrentChar.ToString() + javaFile.PeekNextChar()) && !javaFile.program.EndOfStream)
            {
                javaFile.GetNextChar();
            }

            javaFile.GetNextChar();
        }

        /// <summary>
        /// Loads the Lexeme until the regular expression no longer matches.
        /// </summary>
        public void LoadLexeme(Regex lexemeRegex)
        {
            while (lexemeRegex.IsMatch(CurrentChar.ToString()))
            {
                Lexeme += CurrentChar;
                javaFile.GetNextChar();
            }
        }
        
        /// <summary>
        /// Prints the list of tokens to the console window.
        /// </summary>
        public void Print()
        {
            Console.WriteLine(string.Format("Token          Attribute"));
            Console.WriteLine("------------------------");

            foreach(Token token in tokens)
            {
                Console.WriteLine(string.Format("{0, -15}{1, -10}", token.token, token.attribute));
            }
        }
    }
}
