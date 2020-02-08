using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Scanner : Resources
    {
        public FileHandler javaFile { get; set; }

        public Scanner(FileHandler file)
        {
            javaFile = file;

            // Resources
            Token = Symbol.UnknownT;
            CurrentChar = '\0';
            Lexeme = "";
            Literal = "";
        }

        /// <summary>
        /// Processes tokens until end of stream has been hit.
        /// </summary>
        public void GetNextToken()
        {
            javaFile.SkipWhitespace();

            if (!javaFile.EndOfFile())
            {
                ProcessToken();
            }
            else
            {
                Token = Symbol.EofT;
                Lexeme = "Done!";
                PrintToken();
            }
        }

        /// <summary>
        /// Reads the first token char into Lexeme, which is then
        /// used to determine which function to use for processing.
        /// </summary>
        public void ProcessToken()
        {
            javaFile.GetNextChar();
            Lexeme = CurrentChar.ToString();

            if (commentStartRegex.IsMatch(Lexeme + javaFile.PeekNextChar()))
            {
                ProcessComment();
            }
            else if (char.IsLetter(Lexeme[0]))
            {
                ProcessWordToken();
            }
            else if (char.IsDigit(Lexeme[0]))
            {
                ProcessNumToken();
            }
            else if (comparisonRegex.IsMatch(Lexeme) && lookAheadCharRegex.IsMatch(javaFile.PeekNextChar().ToString()))
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
                Token = Symbol.UnknownT;
                PrintToken();
            }
        }

        /// <summary>
        /// Reads characters into Lexeme, then validates for correct 
        /// word token. Word token can be a reserved word (KeyWords list) 
        /// or an identifier (IdT). Adds token to list.
        /// </summary>
        public void ProcessWordToken()
        {
            LoadLexeme(wordRegex);

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

            PrintToken();
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
            PrintToken();
        }

        /// <summary>
        /// Reads next character into Lexeme, then validates correct 
        /// double token. Adds token to list.
        /// </summary>
        public void ProcessDoubleToken()
        {
            javaFile.GetNextChar();
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

            PrintToken();
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

            PrintToken();
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
            Token = Symbol.QuoteT;
            PrintToken();
            javaFile.GetNextChar();
            Lexeme = CurrentChar.ToString();

            while (javaFile.PeekNextChar() != '\"' && javaFile.PeekNextChar() != '\n' && !javaFile.EndOfFile())
            {
                javaFile.GetNextChar();
                Lexeme += CurrentChar;
            }

            Token = Symbol.LiteralT;
            PrintToken();

            if (javaFile.PeekNextChar() == '\"')
            {
                Token = Symbol.QuoteT;
                javaFile.GetNextChar();
                Lexeme = CurrentChar.ToString();
                PrintToken();
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
            }
            else if (javaFile.PeekNextChar() == '*')
            {
                SkipComment(multiLineCommentEndRegex);
                javaFile.GetNextChar();
                javaFile.GetNextChar();
            }
        }

        /// <summary>
        /// Calls GetNextChar() to skip over comment characters.
        /// </summary>
        public void SkipComment(Regex commentEndRegex)
        {
            while (!commentEndRegex.IsMatch(CurrentChar.ToString() + javaFile.PeekNextChar()) && !javaFile.EndOfFile())
            {
                javaFile.GetNextChar();
            }
        }

        /// <summary>
        /// Loads the Lexeme until the regular expression no longer matches.
        /// </summary>
        public void LoadLexeme(Regex lexemeRegex)
        {
            while (lexemeRegex.IsMatch(javaFile.PeekNextChar().ToString()))
            {
                javaFile.GetNextChar();
                Lexeme += CurrentChar;
            }
        }
        
        /// <summary>
        /// Prints the list of tokens to the console window.
        /// </summary>
        public void PrintToken()
        {
            Console.WriteLine(string.Format("{0, -15}{1, -10}", Token, Lexeme));
        }
    }
}
