using System.Text.RegularExpressions;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class LexicalAnalyzer
    {
        public LexicalAnalyzer()
        {
            // Resources
            Token = Tokens.UnknownT;
            CurrentChar = '\0';
            Lexeme = "";
            Literal = "";
        }

        /// <summary>
        /// Processes tokens until end of stream has been hit.
        /// </summary>
        public void GetNextToken()
        {
            JavaFile.SkipWhitespace();

            if (!JavaFile.EndOfFile())
            {
                ProcessToken();
            }
            else
            {
                Token = Tokens.EofT;
                Lexeme = "End of file";
            }
        }

        /// <summary>
        /// Reads the first token char into Lexeme, which is then
        /// used to determine which function to use for processing.
        /// </summary>
        public void ProcessToken()
        {
            JavaFile.GetNextChar();
            Lexeme = CurrentChar.ToString();

            if (commentStartRegex.IsMatch(Lexeme + JavaFile.PeekNextChar()))
            {
                // skip the comment then continue processing the token
                ProcessComment();
                GetNextToken();
            }
            else if (char.IsLetter(Lexeme[0]))
            {
                ProcessWordToken();
            }
            else if (char.IsDigit(Lexeme[0]))
            {
                ProcessNumToken();
            }
            else if (comparisonRegex.IsMatch(Lexeme) && lookAheadCharRegex.IsMatch(JavaFile.PeekNextChar().ToString()))
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
                Token = Tokens.UnknownT;
            }
        }

        /// <summary>
        /// Reads characters into Lexeme, then validates for correct 
        /// word token. Word token can be a reserved word (KeyWords list) 
        /// or an identifier (IdT).
        /// </summary>
        public void ProcessWordToken()
        {
            LoadLexeme(wordRegex);

            if (Lexeme == "System")
            {
                // continues reading for special case -> System.out.println
                LoadLexeme(printRegex);
                Token = Tokens.PrintT;
            }
            else if (wordRegex.IsMatch(Lexeme) && Lexeme.Length <= 31)
            {
                Token = (KeyWords.Contains(Lexeme)) ? (Tokens)KeyWords.FindIndex(t => t == Lexeme) : Tokens.IdT;
            }
            else
            {
                Token = Tokens.UnknownT;
            }
        }

        /// <summary>
        /// Reads characters into Lexeme, then validates for correct 
        /// number token. Number token (NumT) can be an integer or double 
        /// value.
        /// </summary>
        public void ProcessNumToken()
        {
            LoadLexeme(decimalDigitRegex);
            Token = (numberRegex.IsMatch(Lexeme)) ? Tokens.NumT : Tokens.UnknownT;
        }

        /// <summary>
        /// Reads next character into Lexeme, then validates correct 
        /// double token.
        /// </summary>
        public void ProcessDoubleToken()
        {
            JavaFile.GetNextChar();
            Lexeme += CurrentChar;

            if (CurrentChar == '=') { Token = Tokens.RelOpT; }
            else if (CurrentChar == '&') { Token = Tokens.MulOpT; }
            else if (CurrentChar == '|') { Token = Tokens.AddOpT; }
            else { Token = Tokens.UnknownT; }
        }

        /// <summary>
        /// Validates correct single token.
        /// </summary>
        public void ProcessSingleToken()
        {
            if (Lexeme == "(") { Token = Tokens.LParenT; }
            else if (Lexeme == ")") { Token = Tokens.RParenT; }
            else if (Lexeme == "[") { Token = Tokens.LBrackT; }
            else if (Lexeme == "]") { Token = Tokens.RBrackT; }
            else if (Lexeme == "{") { Token = Tokens.LBraceT; }
            else if (Lexeme == "}") { Token = Tokens.RBraceT; }
            else if (Lexeme == ",") { Token = Tokens.CommaT; }
            else if (Lexeme == ";") { Token = Tokens.SemiT; }
            else if (Lexeme == ".") { Token = Tokens.PeriodT; }
            else if (Lexeme == "=") { Token = Tokens.AssignOpT; }
            else if (Lexeme == "!") { Token = Tokens.NotOpT; }
            else if (Lexeme == "+" || Lexeme == "-") { Token = Tokens.AddOpT; }
            else if (Lexeme == "*" || Lexeme == "/") { Token = Tokens.MulOpT; }
            else if (Lexeme == "<" || Lexeme == ">") { Token = Tokens.RelOpT; }
            else { Token = Tokens.UnknownT; }
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
            Token = Tokens.QuoteT;
            JavaFile.GetNextChar();
            Lexeme = CurrentChar.ToString();

            while (JavaFile.PeekNextChar() != '\"' && JavaFile.PeekNextChar() != '\n' && !JavaFile.EndOfFile())
            {
                JavaFile.GetNextChar();
                Lexeme += CurrentChar;
            }

            Token = Tokens.LiteralT;

            if (JavaFile.PeekNextChar() == '\"')
            {
                Token = Tokens.QuoteT;
                JavaFile.GetNextChar();
                Lexeme = CurrentChar.ToString();
            }
        }

        /// <summary>
        /// Determines type of comment, the comment is then skipped.
        /// </summary>
        public void ProcessComment()
        {
            if (JavaFile.PeekNextChar() == '/')
            {
                SkipComment(oneLineCommentEndRegex);
            }
            else if (JavaFile.PeekNextChar() == '*')
            {
                SkipComment(multiLineCommentEndRegex);
                JavaFile.GetNextChar();
                JavaFile.GetNextChar();
            }
        }

        /// <summary>
        /// Calls GetNextChar() to skip over comment characters.
        /// </summary>
        public void SkipComment(Regex commentEndRegex)
        {
            while (!commentEndRegex.IsMatch(CurrentChar.ToString() + JavaFile.PeekNextChar()) && !JavaFile.EndOfFile())
            {
                JavaFile.GetNextChar();
            }
        }

        /// <summary>
        /// Loads the Lexeme until the regular expression no longer matches.
        /// </summary>
        public void LoadLexeme(Regex lexemeRegex)
        {
            while (lexemeRegex.IsMatch(JavaFile.PeekNextChar().ToString()))
            {
                JavaFile.GetNextChar();
                Lexeme += CurrentChar;
            }
        }
    }
}