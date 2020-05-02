using System;
using System.Collections.Generic;
using System.Text;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class AssemblyGenerator
    {
        public void GenerateASMFile()
        {
            GenerateProgBeginningASM();
            GenerateStartProcASM();
            GenerateProcsASM();

            AssemblyFile.AddLine("END start");
            AssemblyFile.CreateASMFile();
        }

        private void GenerateProgBeginningASM()
        {
            AssemblyFile.AddLine("     .model small");
            AssemblyFile.AddLine("     .586");
            AssemblyFile.AddLine("     .stack 100h");
            AssemblyFile.AddLine("     .data");

            AssemblyFile.AddLiteralsToASMFile();

            AssemblyFile.AddLine("     .code");
            AssemblyFile.AddLine("     include io.asm");
        }

        private void GenerateStartProcASM()
        {
            AssemblyFile.AddLine("");
            AssemblyFile.AddLine("start PROC");
            AssemblyFile.AddLine("     mov ax, @data");
            AssemblyFile.AddLine("     mov ds, ax");
            AssemblyFile.AddLine($"     call {AssemblyFile.mainName}");
            AssemblyFile.AddLine("     mov ah, 04ch");
            AssemblyFile.AddLine("     int 21h");
            AssemblyFile.AddLine("start ENDP");
        }

        private void GenerateProcsASM()
        {
            Method proc = new Method();

            while (ASMProcs.Count > 0)
            {
                proc = ASMProcs.Dequeue();

                AssemblyFile.AddLine("");
                AssemblyFile.AddLine($"{proc.lexeme} PROC");
                AssemblyFile.AddLine("     push bp");
                AssemblyFile.AddLine("     mov bp, sp");
                AssemblyFile.AddLine($"     sub sp, {proc.sizeOfLocalVars}");

                GenerateProcBodyASM();

                AssemblyFile.AddLine($"     add sp, {proc.sizeOfLocalVars}");
                AssemblyFile.AddLine("     pop bp");
                AssemblyFile.AddLine($"     ret {proc.sizeOfParameterVars - 4}");
                AssemblyFile.AddLine($"{proc.lexeme} ENDP");
            }
        }

        private void GenerateProcBodyASM()
        {
            string tacWord = GetAndFormatWord();

            while (tacWord != "endp")
            {
                if (TACKeywords.Contains(tacWord))
                {
                    GenerateProcLineUsingKeywordASM(tacWord);
                }
                else
                {
                    GenerateProcLineUsingVariableASM(tacWord);
                }

                tacWord = GetAndFormatWord();         
            }

            TACFile.GetNextWord();
        }

        private void GenerateProcLineUsingKeywordASM(string tacWord)
        {
            if (tacWord == "proc")
            {
                TACFile.GetNextWord();
            }
            else if (tacWord == "wrs")
            {
                AssemblyFile.AddLine($"     mov dx, offset {TACFile.GetNextWord()}");
                AssemblyFile.AddLine("     call writestr");
            }
            else if (tacWord == "wri")
            {
                string bpWord = GetAndFormatWord();
                AssemblyFile.AddLine($"     mov dx, {bpWord}");
                AssemblyFile.AddLine($"     call writeint");
            }
            else if (tacWord == "wrln")
            {
                AssemblyFile.AddLine($"     call writeln");
            }
            else if (tacWord == "rdi")
            {
                string bpWord = GetAndFormatWord();
                AssemblyFile.AddLine($"     call readint");
                AssemblyFile.AddLine($"     mov {bpWord}, bx");
            }
            else if (tacWord == "call")
            {
                AssemblyFile.AddLine($"     call {TACFile.GetNextWord()}");
            }
            else if (tacWord == "push")
            {
                AssemblyFile.AddLine($"     mov ax, {GetAndFormatWord()}");
                AssemblyFile.AddLine($"     push ax");
            }
        }

        private void GenerateProcLineUsingVariableASM(string tacWord)
        {
            if (tacWord == "_ax")
            {
                TACFile.GetNextWord();
                AssemblyFile.AddLine($"     mov ax, {GetAndFormatWord()}");
            }
            else
            {
                TACFile.GetNextWord(); // skip equal sign
                string axReg = GetAndFormatWord();
                char opChar = TACFile.PeekNextChar();

                if (opChar == '*' || opChar == '/' || opChar == '-' || opChar == '+')
                {
                    GenerateOperationLineASM(opChar, axReg, tacWord);
                }
                else
                {
                    GenerateAssignmentLineASM(axReg, tacWord);
                }
            }
        }

        private void GenerateOperationLineASM(char opChar, string axReg, string tacWord)
        {
            TACFile.GetNextWord(); // skip operator

            if (opChar == '+')
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
                AssemblyFile.AddLine($"     add ax, {GetAndFormatWord()}");
                AssemblyFile.AddLine($"     mov {tacWord}, ax");
            }
            else if (opChar == '-')
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
                AssemblyFile.AddLine($"     sub ax, {GetAndFormatWord()}");
                AssemblyFile.AddLine($"     mov {tacWord}, ax");
            }
            else if (opChar == '/')
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
                AssemblyFile.AddLine($"     mov bx, {GetAndFormatWord()}");
                AssemblyFile.AddLine($"     idiv bx");
                AssemblyFile.AddLine($"     mov {tacWord}, ax");
            }
            else if (opChar == '*')
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
                AssemblyFile.AddLine($"     mov bx, {GetAndFormatWord()}");
                AssemblyFile.AddLine($"     imul bx");
                AssemblyFile.AddLine($"     mov {tacWord}, ax");
            }
        }

        private void GenerateAssignmentLineASM(string axReg, string tacWord)
        {
            if (axReg.Contains("ax"))
            {
                AssemblyFile.AddLine($"     mov {tacWord}, ax");
            }
            else if (tacWord.Contains("ax"))
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
            }
            else
            {
                AssemblyFile.AddLine($"     mov ax, {axReg}");
                AssemblyFile.AddLine($"     mov {tacWord}, ax");

                if (!axReg.Contains("bp"))
                {
                    tacWord = GetAndFormatWord();
                    TACFile.GetNextWord(); // skip equal sign
                    AssemblyFile.AddLine($"     mov ax, {GetAndFormatWord()}");
                    AssemblyFile.AddLine($"     mov {tacWord}, ax");
                }
            }
        }

        private string GetAndFormatWord()
        {
            string word = TACFile.GetNextWord();

            if (word[0] == '_')
            {
                word = word.Replace('_', '[');
                word += ']';
            }

            return word;
        }
    }
}