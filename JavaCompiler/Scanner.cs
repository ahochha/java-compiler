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

            //Resources
            Token = Symbol.UnknownT;
            Lexeme = "";
            Literal = "";
            CurrentChar = ' ';
            Value = 0;
            ValueR = 0.0;
        }

        public void GetNextChar()
        {
            CurrentChar = javaFile.currentLine[javaFile.charIndex];
            javaFile.charIndex++;
        }

        public void GetNextToken()
        {
            if (javaFile.lineNum <= javaFile.lines.Count())
            {
                if (!javaFile.lines[javaFile.lineNum].EndsWith(CurrentChar))
                {
                    GetNextChar();

                    while (CurrentChar == ' ')
                    {
                        GetNextChar();
                    }

                    ProcessToken();
                }
                else
                {
                    javaFile.lineNum++;
                    javaFile.charIndex = 0;
                    javaFile.SetCurrentLine(javaFile.lineNum);
                }
            }
            else
            {
                Token = Symbol.EofT;
            }
        }

        public void ProcessToken()
        {
            Regex letter = new Regex(@"[a-zA-Z]");
            Regex number = new Regex(@"[0-9]");
            Regex literal = new Regex("(\")");
            Regex comparison = new Regex(@"<|>|!|=");
            Regex comment = new Regex(@"/");
            Lexeme = CurrentChar.ToString();
            GetNextChar();

            if (letter.IsMatch(Lexeme))
            {
                ProcessWordToken();
            }
            else if (number.IsMatch(Lexeme))
            {
                ProcessNumToken();
            }
            else if (literal.IsMatch(Lexeme))
            {
                ProcessLiteral();
            }
            else if (comment.IsMatch(Lexeme))
            {
                ProcessCommentToken();
            }
        }

        public void ProcessWordToken()
        {
            Regex word = new Regex(@"\w+");
            Symbol token = Symbol.UnknownT;

            while (word.IsMatch(CurrentChar.ToString()))
            {
                Lexeme += CurrentChar;
                GetNextChar();
            }

            if (word.IsMatch(Lexeme))
            {
                token = (KeyWords.Contains(Lexeme)) ? (Symbol)KeyWords.FindIndex(t => KeyWords.Contains(Lexeme)) : Symbol.IdT;
                tokens.Add(new Token(token, Lexeme));
            }
            else
            {
                //log error for invalid word token
            }
        }

        public void ProcessNumToken()
        {
            Regex number = new Regex(@"[0-9]");

            while (number.IsMatch(CurrentChar.ToString()))
            {
                Lexeme += CurrentChar;
                GetNextChar();
            }
        }

        public void ProcessLiteral()
        {

        }

        public void ProcessCommentToken()
        {

        }

        public void ProcessDoubleToken()
        {

        }

        public void ProcessSingleToken()
        {

        }
    }
}
