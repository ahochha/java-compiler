using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaCompiler
{
    public class Scanner : Resources
    {
        public Scanner()
        {
            Token = Symbol.UnknownT;
            Lexeme = "";
            LineNum = 1;
            GetNextChar();
        }

        public void GetNextChar()
        {
            CurrentChar = FileHandler.CurrentLine[FileHandler.CharNum];
            FileHandler.CharNum++;
        }

        public void GetNextToken()
        {
            while (CurrentChar == ' ')
            {
                GetNextChar();
            }

            if (LineNum <= FileHandler.Lines.Count())
            {
                if (!FileHandler.Lines[LineNum].EndsWith(CurrentChar))
                {
                    ProcessToken();
                }
                else
                {
                    LineNum++;
                    FileHandler.CharNum = 0;
                    FileHandler.CurrentLine = FileHandler.Lines[LineNum];
                }
            }
            else
            {
                Token = Symbol.EofT;
            }
        }

        public void ProcessToken()
        {
            Regex Letter = new Regex(@"[a-zA-Z]");
            Regex Number = new Regex(@"[0-9]");
            Regex Comparison = new Regex(@"<|>|=");
            Regex Comment = new Regex(@"/");
            Lexeme = CurrentChar.ToString();
            GetNextChar();

            if (Letter.IsMatch(Lexeme))
            {
                ProcessWordToken();
            }
            else if (Number.IsMatch(Lexeme))
            {
                ProcessNumToken();
            }
            else if (Comment.IsMatch(Lexeme))
            {
                //...
            }
        }

        public void ProcessWordToken()
        {

        }

        public void ProcessNumToken()
        {

        }

        public void ProcessComment()
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
