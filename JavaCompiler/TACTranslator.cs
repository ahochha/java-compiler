using System;
using System.Collections.Generic;
using System.Text;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class TACTranslator
    {
        public int tempVarOffset { get; set; }

        public TACTranslator()
        {
            tempVarOffset = 2;
        }

        /// <summary>
        /// Adds the generated code to the list of lines. Line is passed by reference and is
        /// reset after being added.
        /// </summary>
        public void GenerateLineOfTAC(ref string line)
        {
            TACFile.AddLine(line);
            line = "";
        }

        /// <summary>
        /// Adds the generated code to the list of lines. Line is passed by value.
        /// </summary>
        public void GenerateLineOfTAC(string line)
        {
            TACFile.AddLine(line);
        }

        /// <summary>
        /// Generates the code that moves the variable to a temp variable.
        /// Generates the _bp-6 = 5 line shown below.
        /// Ex. a = 5 **a is represented by _bp-2, _bp-6 is the temp var**
        ///     _bp-6 = 5
        ///     _bp-2 = _bp-6
        /// </summary>
        public void GenerateTempExpressionTAC(ref ITableEntry Tplace)
        {
            string tempVarName = "";
            NewTempVar(ref tempVarName, Tplace);
            GenerateLineOfTAC($"{tempVarName} = {Tplace.bpOffsetName}");
            Tplace.bpOffsetName = tempVarName;
        }

        /// <summary>
        /// Generates a segment from a complex statement. Will be called while recursively 
        /// traversing the grammar.
        /// Generates _bp-6 = 4 + on the first call in the example shown below.
        /// Ex. a = 4 + (3 + a * b) **assume _bp-6 is a generated temp var**
        ///     _bp-6 = 4 +
        ///     ... (process continues)...
        /// </summary>
        public void GenerateSegmentOfExpressionTAC(ref string code, string tempVarName, ITableEntry Rplace)
        {
            code = $"{tempVarName} = {Rplace.bpOffsetName} {Lexeme} ";
        }

        /// <summary>
        /// Generates the final line of TAC for the expression. This is where the expression gets
        /// assigned to the variable that holds the calculated value.
        /// Ex. a = 4 + (3 + a * b) **assume _bp-10 is the temp var that holds the calculated value
        /// and _bp-2 represents a**
        ///     _bp-2 = _bp-10
        /// </summary>
        public void GenerateFinalExpressionTAC(ITableEntry IdEntry, ITableEntry Eplace, ref string code)
        {
            code = $"{IdEntry.bpOffsetName} = {Eplace.bpOffsetName}";
            GenerateLineOfTAC(ref code);
        }

        /// <summary>
        /// Generates a new temp var for use while building TAC. Increments the tempVarOffset 
        /// to prepare for the next temp var.
        /// </summary>
        public void NewTempVar(ref string tempVarName, ITableEntry entry)
        {
            tempVarName = $"_bp-{LocalVarsSize + tempVarOffset}";

            if (entry != null && entry.typeOfEntry == EntryType.varEntry)
            {
                Variable var = entry as Variable;

                if (var.size != 0)
                {
                    tempVarOffset += var.size;
                }
                else
                {
                    var.size = GetDirectValue(entry);
                }
            }
            else if (entry.typeOfEntry == EntryType.constEntry)
            {
                int num = GetDirectValue(entry);
            }
        }

        /// <summary>
        /// Finds the amount to increment tempVarOffset for a non variable segment of the expression.
        /// Ex. The number 5 would result in adding 2 to tempVarOffset due to being an integer value.
        /// </summary>
        public int GetDirectValue(ITableEntry entry)
        {
            int number = 0;

            if (float.TryParse(entry.bpOffsetName, out float valueR) && entry.bpOffsetName.Contains("."))
            {
                tempVarOffset += 4;
                number = 4;
            }
            else if (int.TryParse(entry.bpOffsetName, out int value))
            {
                tempVarOffset += 2;
                number = 2;
            }
            else if (entry.bpOffsetName == "true" || entry.bpOffsetName == "false") {
                tempVarOffset += 1;
                number = 1;
            }
            else
            {
                ErrorHandler.LogError($"Invalid value provided in expression.");
            }

            return number;
        }
    }
}
