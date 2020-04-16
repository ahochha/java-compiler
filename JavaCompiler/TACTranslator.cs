using System;
using System.Collections.Generic;
using System.Text;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class TACTranslator
    {
        public int tempVarOffset { get; set; }
        public string directValue { get; set; }

        public TACTranslator()
        {
            tempVarOffset = 2;
        }

        public void GenerateLineOfTAC(ref string line)
        {
            TACFile.AddLine(line);
            line = "";
        }

        public void GenerateLineOfTAC(string line)
        {
            TACFile.AddLine(line);
        }

        public void GenerateTempExpressionTAC(ref ITableEntry Tplace)
        {
            string tempVarName = "";
            NewTempVar(ref tempVarName, Tplace);
            GenerateLineOfTAC($"{tempVarName} = {Tplace.bpOffsetName}");
            Tplace.bpOffsetName = tempVarName;
        }

        public void GenerateSegmentOfExpressionTAC(ref string code, string tempVarName, ITableEntry Rplace)
        {
            code = $"{tempVarName} = {Rplace.bpOffsetName} {Lexeme} ";
        }

        public void GenerateFinalExpressionTAC(ITableEntry IdEntry, ITableEntry Eplace, ref string code)
        {
            code = $"{IdEntry.bpOffsetName} = {Eplace.bpOffsetName}";
            GenerateLineOfTAC(ref code);
        }

        public void NewTempVar(ref string tempVarName, ITableEntry entry)
        {
            Variable var = entry as Variable;
            tempVarName = $"_bp-{LocalVarsSize + tempVarOffset}";

            if (var != null && var.size != 0)
            {
                tempVarOffset += var.size;
            }
            else
            {
                GetDirectValue(var);
            }
        }

        public void GetDirectValue(Variable var)
        {
            int number = 0;

            if (int.TryParse(var.bpOffsetName, out number) && var.bpOffsetName.Contains("."))
            {
                var.size = 4;
                tempVarOffset += 4;
            }
            else if (int.TryParse(var.bpOffsetName, out number))
            {
                var.size = 2;
                tempVarOffset += 2;
            }
            else if (var.bpOffsetName == "true" || var.bpOffsetName == "false") {
                var.size = 1;
                tempVarOffset += 1;
            }
            else
            {
                ErrorHandler.LogError($"Invalid value provided in expression.");
            }
        }
    }
}
