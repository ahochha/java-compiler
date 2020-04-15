using System;
using System.Collections.Generic;
using System.Text;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class TACTranslator
    {
        public string code { get; set; }
        public string tempVarName { get; set; }
        public int tempVarOffset { get; set; }
        public bool hasSegments { get; set; }
        public Variable Eplace { get; set; }
        public Variable Tplace { get; set; }
        public Variable Rplace { get; set; }
        public Stack<string> bpStack { get; set; }
        public Stack<string> tempVarStack { get; set; }
        public Stack<Stack<string>> exprStack { get; set; }

        public TACTranslator()
        {
            code = "";
            tempVarName = "";
            tempVarOffset = 2;
            bpStack = new Stack<string>();
            tempVarStack = new Stack<string>();
            exprStack = new Stack<Stack<string>>();
        }

        public void GenerateLineOfTAC(string line)
        {
            TACFile.AddLine(line);
            code = "";
        }

        //public void GenerateTempSegmentOfExpressionTAC()
        //{
        //    NewTempVar();
        //    code = $"{tempVarName} = {Eplace.lexeme}";
        //    GenerateLineOfTAC(code);
        //}

        public void GenerateSegmentOfExpressionTAC()
        {
            code = $"{tempVarName} = {Rplace.bpOffsetVarName} {Lexeme} ";
        }

        public void GenerateFinalExpressionTAC(Variable var)
        {
            if (hasSegments == true)
            {
                tempVarName = TACFile.GetFinalVarName();
            }

            code = $"{var.bpOffsetVarName} = {tempVarName}";
            GenerateLineOfTAC(code);
            hasSegments = false;
        }

        public void NewTempVar()
        {
            tempVarName = $"_bp-{LocalVarsSize + tempVarOffset}";

            if (Tplace.size != 0)
            {
                tempVarOffset += Tplace.size;
            }
            else
            {
                GetDirectValue();
            }
        }

        public void GetDirectValue()
        {
            int number = 0;

            if (int.TryParse(Tplace.lexeme, out number) && Tplace.lexeme.Contains("."))
            {
                tempVarOffset += 4;
            }
            else if (int.TryParse(Tplace.lexeme, out number))
            {
                tempVarOffset += 2;
            }
            else if (Tplace.lexeme == "true" || Tplace.lexeme == "false") {
                tempVarOffset += 1;
            }
            else
            {
                ErrorHandler.LogError($"Invalid value provided in expression.");
            }
        }
    }
}
