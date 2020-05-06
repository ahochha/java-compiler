using System.Collections.Generic;
using System.Linq;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class Parser
    {
        private LexicalAnalyzer lexicalAnalyzer { get; set; }
        public SymbolTable symbolTable { get; set; }
        public TACGenerator tacGenerator { get; set; }

        public Parser()
        {
            lexicalAnalyzer = new LexicalAnalyzer();
            symbolTable = new SymbolTable();
            tacGenerator = new TACGenerator();

            lexicalAnalyzer.GetNextToken();
        }

        /// <summary>
        /// Checks for a match between the current token and the desired token.
        /// </summary>
        private void Match(Tokens desired)
        {
            if (Token == desired)
            {
                lexicalAnalyzer.GetNextToken();
            }
            else
            {
                ErrorHandler.LogError(desired);
            }
        }

        /// <summary>
        /// Prog -> MoreClasses MainClass
        /// </summary>
        public void Prog()
        {
            MoreClasses();
            MainClass();
            symbolTable.DeleteDepth(Depth);
            TACFile.CreateTACFile();
        }

        /// <summary>
        /// MoreClasses -> ClassDecl MoreClasses | ε
        /// </summary>
        private void MoreClasses()
        {
            if (Token == Tokens.ClassT)
            {
                ClassDecl();
                MoreClasses();
            }
            else if (Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected class declaration, found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// MainClass -> FinalT ClassT IdT { 
        ///                 PublicT StaticT VoidT MainT ( StringT [ ] IdT ) { 
        ///                     SeqOfStatements 
        ///                 }
        ///              }
        /// </summary>
        private void MainClass()
        {
            Match(Tokens.FinalT);
            Match(Tokens.ClassT);
            TableEntry finalClassEntry = symbolTable.CreateTableEntry();
            Match(Tokens.IdT);
            Depth++;
            ResetClassGlobals();
            Match(Tokens.LBraceT);
            Match(Tokens.PublicT);
            Match(Tokens.StaticT);
            Match(Tokens.VoidT);
            TypeReturn = VarType.voidType;
            TableEntry mainEntry = symbolTable.CreateTableEntry();
            MethodNames.Add(Lexeme);
            tacGenerator.GenerateLineOfTAC("proc main");
            AssemblyFile.mainName = Lexeme;
            Match(Tokens.MainT);
            Depth++;
            ResetMethodGlobals();
            Match(Tokens.LParenT);
            Match(Tokens.StringT);
            Match(Tokens.LBrackT);
            Match(Tokens.RBrackT);
            Match(Tokens.IdT);
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            SeqOfStatements();
            tacGenerator.GenerateLineOfTAC("endp main");
            ParameterVarsSize = 4;
            symbolTable.ConvertEntryToMethod(mainEntry);
            symbolTable.DeleteDepth(Depth);
            Depth--;
            Match(Tokens.RBraceT);
            symbolTable.ConvertEntryToClass(finalClassEntry);
            symbolTable.DeleteDepth(Depth);
            Depth--;
            Match(Tokens.RBraceT);
        }

        /// <summary>
        /// ClassDecl -> ClassT IdT { VarDecl MethodDecl } |
        ///              ClassT IdT ExtendsT IdT { VarDecl MethodDecl } |
        ///              ε
        /// </summary>
        private void ClassDecl()
        {
            int tempLocalVarsSize;
            List<string> tempVarNames = new List<string>();

            Match(Tokens.ClassT);
            TableEntry entry = symbolTable.CreateTableEntry();
            Match(Tokens.IdT);

            if (Token == Tokens.ExtendsT)
            {
                Match(Tokens.ExtendsT);
                Match(Tokens.IdT);
            }

            Depth++;
            ResetClassGlobals();
            Match(Tokens.LBraceT);
            VarDecl();
            tempLocalVarsSize = LocalVarsSize;
            tempVarNames = VarNames;
            MethodDecl();
            LocalVarsSize = tempLocalVarsSize;
            VarNames = tempVarNames;
            symbolTable.ConvertEntryToClass(entry);
            symbolTable.DeleteDepth(Depth);
            Depth--;
            Match(Tokens.RBraceT);
        }

        /// <summary>
        /// VarDecl -> Type IdentifierList ; VarDecl |
        ///            FinalT Type IdT = NumT ; VarDecl |
        ///            ε
        /// </summary>
        private void VarDecl()
        {
            if (Token == Tokens.FinalT)
            {
                Match(Tokens.FinalT);
                Type();
                TableEntry entry = symbolTable.CreateTableEntry();
                VarNames.Add(Lexeme);
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);
                BpOffsetName = $"_bp-{Offset}";
                symbolTable.ConvertEntryToConstant(entry);
                Match(Tokens.NumT);
                LocalVarsSize += Size;
                Offset += Size;
                Match(Tokens.SemiT);
                VarDecl();
            }
            else if (Types.Contains(Token))
            {
                Type();
                IdentifierList();
                Match(Tokens.SemiT);
                VarDecl();
            }
        }

        /// <summary>
        /// Type -> IntT | FloatT | BooleanT | VoidT
        /// </summary>
        private void Type()
        {
            if (Token == Tokens.IntT)
            {
                TypeVar = VarType.intType;
                TypeConst = VarType.intType;
                Size = 2;
                Match(Tokens.IntT);
            }
            else if (Token == Tokens.FloatT)
            {
                TypeVar = VarType.floatType;
                TypeConst = VarType.floatType;
                Size = 4;
                Match(Tokens.FloatT);
            }
            else if (Token == Tokens.BooleanT)
            {
                TypeVar = VarType.booleanType;
                Size = 1;
                Match(Tokens.BooleanT);
            }
            else if (Token == Tokens.VoidT)
            {
                TypeVar = VarType.voidType;
                Size = 0;
                Match(Tokens.VoidT);
            }
            else
            {
                ErrorHandler.LogError($"expected type declaration, found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// IdentifierList -> IdT IdentifierListTail
        /// </summary>
        private void IdentifierList()
        {
            if (Token == Tokens.IdT)
            {
                TableEntry entry = symbolTable.CreateTableEntry();
                BpOffsetName = $"_bp-{Offset}";
                symbolTable.ConvertEntryToVariable(entry);
                VarNames.Add(Lexeme);
                LocalVarsSize += Size;
                Offset += Size;
                Match(Tokens.IdT);
                IdentifierListTail();
            }
            else
            {
                ErrorHandler.LogError($"expected an identifier, found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// IdentifierListTail -> , IdT IdentifierListTail | ε
        /// </summary>
        private void IdentifierListTail()
        {
            if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                TableEntry entry = symbolTable.CreateTableEntry();
                BpOffsetName = $"_bp-{Offset}";
                symbolTable.ConvertEntryToVariable(entry);
                VarNames.Add(Lexeme);
                LocalVarsSize += Size;
                Offset += Size;
                Match(Tokens.IdT);

                if (Token == Tokens.IdT)
                {
                    ErrorHandler.LogError("expected \",\" before next identifier");
                }

                IdentifierListTail();
            }
        }

        /// <summary>
        /// MethodDecl -> PublicT Type IdT ( FormalList ) {
        ///                   VarDecl SeqOfStatements ReturnT Expr ;
        ///               } | 
        ///               ε               
        /// </summary>
        private void MethodDecl()
        {
            ITableEntry Eplace = null;

            if (Token == Tokens.PublicT)
            {
                Match(Tokens.PublicT);
                Type();
                TypeReturn = TypeVar;
                TableEntry entry = symbolTable.CreateTableEntry();
                MethodNames.Add(Lexeme);
                tacGenerator.GenerateLineOfTAC($"proc {entry.lexeme}");
                Match(Tokens.IdT);
                Depth++;
                ResetMethodGlobals();
                Match(Tokens.LParenT);
                ParameterVarsSize = 4;
                FormalList();
                Offset = 2;
                tacGenerator.tempVarOffset = 2;
                Match(Tokens.RParenT);
                Match(Tokens.LBraceT);
                VarDecl();
                SeqOfStatements();
                Match(Tokens.ReturnT);
                Expr(ref Eplace);

                if (Eplace != null && Eplace.bpOffsetName != "")
                {
                    tacGenerator.GenerateLineOfTAC($"_ax = {Eplace.bpOffsetName}");
                }

                Match(Tokens.SemiT);
                symbolTable.ConvertEntryToMethod(entry);
                symbolTable.DeleteDepth(Depth);
                tacGenerator.GenerateLineOfTAC($"endp {entry.lexeme}");
                tacGenerator.GenerateLineOfTAC("");
                Depth--;
                Match(Tokens.RBraceT);
                MethodDecl();
            }
        }

        /// <summary>
        /// FormalList -> Type IdT FormalRest | ε
        /// </summary>
        private void FormalList()
        {
            if (Types.Contains(Token))
            {
                Type();
                TableEntry entry = symbolTable.CreateTableEntry();
                BpOffsetName = $"_bp+{ParameterVarsSize}";
                symbolTable.ConvertEntryToVariable(entry);
                ParameterNames.Add(Lexeme);
                ParameterTypes.Add(TypeVar);
                ParameterNum++;
                ParameterVarsSize += Size;
                Offset += Size;
                Match(Tokens.IdT);
                FormalRest();
            }
            else if (Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected type declaration, found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// FormalRest -> , Type IdT FormalRest | ε
        /// </summary>
        private void FormalRest()
        {
            if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Type();
                TableEntry entry = symbolTable.CreateTableEntry();
                BpOffsetName = $"_bp+{ParameterVarsSize}";
                symbolTable.ConvertEntryToVariable(entry);
                ParameterTypes.Add(TypeVar);
                ParameterNum++;
                ParameterVarsSize += Size;
                Offset += Size;
                Match(Tokens.IdT);
                FormalRest();
            }
            else if (Types.Contains(Token) || Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected \",\", found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// SeqOfStatements -> Statement ; SeqOfStatements | ε
        /// </summary>
        private void SeqOfStatements()
        {
            if (Token == Tokens.IdT || Token == Tokens.WriteT || Token == Tokens.WritelnT || Token == Tokens.ReadT)
            {
                Statement();
                Match(Tokens.SemiT);
                SeqOfStatements();
            }
        }

        /// <summary>
        /// Statement -> AssignStat | IOStat
        /// </summary>
        private void Statement()
        {
            if (Token == Tokens.IdT) {
                AssignStat();
            }
            else if (Token == Tokens.WriteT || Token == Tokens.WritelnT || Token == Tokens.ReadT)
            {
                IOStat();
            }
        }

        /// <summary>
        /// AssignStat -> IdT = Expr | IdT = MethodCall | MethodCall
        /// </summary>
        private void AssignStat()
        {
            ITableEntry firstIdEntry = symbolTable.Lookup(Lexeme);
            ITableEntry Eplace = null;
            string code = "";

            if (firstIdEntry != null && (firstIdEntry.typeOfEntry == EntryType.classEntry || firstIdEntry.typeOfEntry == EntryType.tableEntry))
            {
                MethodCall(firstIdEntry as Class);
            }
            else if (firstIdEntry != null && firstIdEntry.typeOfEntry == EntryType.varEntry)
            {
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);

                ITableEntry secondIdEntry = symbolTable.Lookup(Lexeme);

                if (secondIdEntry != null && (secondIdEntry.typeOfEntry == EntryType.classEntry || secondIdEntry.typeOfEntry == EntryType.tableEntry))
                {
                    MethodCall(secondIdEntry as Class);
                    tacGenerator.GenerateLineOfTAC($"{firstIdEntry.bpOffsetName} = _ax");
                }
                else 
                {
                    Expr(ref Eplace);
                    tacGenerator.GenerateFinalExpressionTAC(firstIdEntry, Eplace, ref code);
                }
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }
        }

        /// <summary>
        /// Expr -> Relation | ε
        /// </summary>
        private void Expr(ref ITableEntry Eplace)
        {
            ITableEntry Tplace = null;

            if (FactorTokens.Contains(Token))
            {
                Relation(ref Tplace);
                Eplace = Tplace;
            }
        }

        /// <summary>
        /// Relation -> SimpleExpr
        /// </summary>
        private void Relation(ref ITableEntry Tplace)
        {
            SimpleExpr(ref Tplace);
        }

        /// <summary>
        /// SimpleExpr -> Term MoreTerm
        /// </summary>
        private void SimpleExpr(ref ITableEntry Tplace)
        {
            Term(ref Tplace);
            MoreTerm(ref Tplace);
        }

        /// <summary>
        /// MoreTerm -> AddOpT Term MoreTerm | ε
        /// </summary>
        private void MoreTerm(ref ITableEntry Rplace)
        {
            string code = "";
            string tempVarName = "";
            ITableEntry Tplace = null;

            if (Token == Tokens.AddOpT)
            {
                //generates a new var then creates the first part of the line
                tacGenerator.NewTempVar(ref tempVarName, Rplace);
                tacGenerator.GenerateSegmentOfExpressionTAC(ref code, tempVarName, Rplace);
                AddOp();
                Term(ref Tplace);
                //finishes the line after the recursive call, using the info gathered from recursion
                code += Tplace.bpOffsetName;
                Rplace.bpOffsetName = tempVarName;
                tacGenerator.GenerateLineOfTAC(ref code);
                MoreTerm(ref Rplace);
            }
        }

        /// <summary>
        /// Term -> Factor MoreFactor
        /// </summary>
        private void Term(ref ITableEntry Tplace)
        {
            Factor(ref Tplace);
            MoreFactor(ref Tplace);
        }
           

        /// <summary>
        /// MoreFactor -> MulOpT Factor MoreFactor | ε
        /// </summary>
        private void MoreFactor(ref ITableEntry Rplace)
        {
            string code = "";
            string tempVarName = "";
            ITableEntry Tplace = null;

            if (Token == Tokens.MulOpT)
            {
                //generates a new var then creates the first part of the line
                tacGenerator.NewTempVar(ref tempVarName, Rplace);
                tacGenerator.GenerateSegmentOfExpressionTAC(ref code, tempVarName, Rplace);
                MulOp();
                Factor(ref Tplace);
                //finishes the line after the recursive call, using the info gathered from recursion
                code += Tplace.bpOffsetName;
                Rplace.bpOffsetName = tempVarName;
                tacGenerator.GenerateLineOfTAC(ref code);
                MoreFactor(ref Rplace);
            }
        }

        /// <summary>
        /// Factor - > IdT | NumT | (Expr) | ! Factor | SignOp Factor | TrueT | FalseT
        /// </summary>
        private void Factor(ref ITableEntry Tplace)
        {
            if (Token == Tokens.IdT)
            {
                ITableEntry entry = symbolTable.Lookup(Lexeme);

                if (entry.typeOfEntry == EntryType.varEntry)
                {
                    Tplace = new Variable(entry as Variable);
                }
                else
                {
                    Tplace = new Constant(entry as Constant);
                    Constant constVar = Tplace as Constant;
                    Tplace.bpOffsetName = constVar.value.ToString();
                }

                if (Tplace != null)
                {
                    Match(Tokens.IdT);
                }
                else
                {
                    ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
                }
            }
            else if (Token == Tokens.NumT)
            {
                Tplace = new Variable() { bpOffsetName = Lexeme, typeOfEntry = EntryType.varEntry };
                tacGenerator.GenerateTempExpressionTAC(ref Tplace);
                Match(Tokens.NumT);
            }
            else if (Token == Tokens.LParenT)
            {
                Match(Tokens.LParenT);
                Expr(ref Tplace);
                Match(Tokens.RParenT);
            }
            else if (Token == Tokens.NotOpT)
            {
                Match(Tokens.NotOpT);
                Factor(ref Tplace);

                Tplace.bpOffsetName = $"!{Tplace.bpOffsetName}";
            }
            else if (Token == Tokens.AddOpT && Lexeme == "-")
            {
                string code = "";
                string tempVarName = "";

                // negates the value returned from the recursion
                // Ex. _bp-2 = 0 - _bp-4
                // can only handle integers right now
                Tplace = new Variable() { size = 2, typeOfEntry = EntryType.varEntry, bpOffsetName = "0" };
                tacGenerator.NewTempVar(ref tempVarName, Tplace);
                tacGenerator.GenerateSegmentOfExpressionTAC(ref code, tempVarName, Tplace);
                SignOp();
                Factor(ref Tplace);
                code += Tplace.bpOffsetName;
                Tplace.bpOffsetName = tempVarName;
                tacGenerator.GenerateLineOfTAC(ref code);
            }
            else if (Token == Tokens.TrueT)
            {
                Tplace = new Variable() { bpOffsetName = Lexeme, typeOfEntry = EntryType.varEntry };
                tacGenerator.GenerateTempExpressionTAC(ref Tplace);
                Match(Tokens.TrueT);
            }
            else if (Token == Tokens.FalseT)
            {
                Tplace = new Variable() { bpOffsetName = Lexeme, typeOfEntry = EntryType.varEntry };
                tacGenerator.GenerateTempExpressionTAC(ref Tplace);
                Match(Tokens.FalseT);
            }
            else
            {
                ErrorHandler.LogError($"expected valid expression, failed at \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// AddOp -> + | - | ||
        /// </summary>
        private void AddOp()
        {
            Match(Tokens.AddOpT);
        }

        /// <summary>
        /// MulOp -> * | / | &&
        /// </summary>
        private void MulOp()
        {
            Match(Tokens.MulOpT);
        }

        /// <summary>
        /// SignOp -> -
        /// </summary>
        private void SignOp()
        {
            Match(Tokens.AddOpT);
        }

        /// <summary>
        /// MethodClass -> ClassName.IdT(Params)
        /// </summary>
        private void MethodCall(Class classEntry)
        {
            ClassName();
            Match(Tokens.PeriodT);

            if ((classEntry != null && classEntry.methodNames.Contains(Lexeme)) || MethodNames.Contains(Lexeme))
            {
                string methodName = Lexeme;
                Match(Tokens.IdT);
                Match(Tokens.LParenT);
                Params();
                Match(Tokens.RParenT);
                tacGenerator.GenerateLineOfTAC($"call {methodName}");
            }
            else
            {
                ErrorHandler.LogError($"The class \"{classEntry.lexeme}\" does not contain the method \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// ClassName -> IdT
        /// </summary>
        private void ClassName()
        {
            Match(Tokens.IdT);
        }

        /// <summary>
        /// Params -> IdT ParamsTail | NumT ParamsTail
        /// </summary>
        private void Params()
        {
            if (Token == Tokens.IdT)
            {
                ParamsHelper();
            }
            else if (Token == Tokens.NumT || Token == Tokens.TrueT || Token == Tokens.FalseT)
            {
                tacGenerator.GenerateLineOfTAC($"push {Lexeme}");

                if (Token == Tokens.NumT)
                {
                    Match(Tokens.NumT);
                }
                else if (Token == Tokens.TrueT)
                {
                    Match(Tokens.TrueT);
                }
                else
                {
                    Match(Tokens.FalseT);
                }

                ParamsTail();
            }
            else if (Token != Tokens.RParenT)
            {
                ErrorHandler.LogError($"parameters to method calls must be variables, numbers, true, or false");
            }
        }

        /// <summary>
        /// ParamsTail -> CommaT IdT ParamsTail | CommaT NumT ParamsTail | ε
        /// </summary>
        private void ParamsTail()
        {
            if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);

                if (Token == Tokens.IdT)
                {
                    ParamsHelper();
                }
                else if (Token == Tokens.NumT || Token == Tokens.TrueT || Token == Tokens.FalseT)
                {
                    tacGenerator.GenerateLineOfTAC($"push {Lexeme}");

                    if (Token == Tokens.NumT)
                    {
                        Match(Tokens.NumT);
                    }
                    else if (Token == Tokens.TrueT)
                    {
                        Match(Tokens.TrueT);
                    }
                    else
                    {
                        Match(Tokens.FalseT);
                    }

                    ParamsTail();
                }
                else
                {
                    ErrorHandler.LogError($"parameters to method calls must be variables, numbers, true, or false");
                }
            }
        }

        /// <summary>
        /// Verifies method variable.
        /// </summary>
        private void ParamsHelper()
        {
            ITableEntry entry = symbolTable.Lookup(Lexeme);

            if (entry != null)
            {
                Variable var = entry as Variable;
                Match(Tokens.IdT);
                ParamsTail();
                tacGenerator.GenerateLineOfTAC($"push {var.bpOffsetName}");
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }
        }

        /// <summary>
        /// IOStat -> InStat | OutStat
        /// </summary>
        private void IOStat()
        {
            if (Token == Tokens.ReadT)
            {
                InStat();
            }
            else if (Token == Tokens.WriteT || Token == Tokens.WritelnT)
            {
                OutStat();
            }
        }

        /// <summary>
        /// InStat -> read ( IdList )
        /// </summary>
        private void InStat()
        {
            Match(Tokens.ReadT);
            Match(Tokens.LParenT);
            IdList();
            Match(Tokens.RParenT);
        }

        /// <summary>
        /// IdList -> IdT IdListTail
        /// </summary>
        private void IdList()
        {
            ITableEntry entry = symbolTable.Lookup(Lexeme);

            if (entry != null)
            {
                tacGenerator.GenerateLineOfTAC($"rdi {entry.bpOffsetName}");
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }

            Match(Tokens.IdT);
            IdListTail();
        }

        /// <summary>
        /// IdListTail -> , IdT IdListTail | ε
        /// </summary>
        private void IdListTail()
        {
            if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Match(Tokens.IdT);
                IdListTail();
            }
        }

        /// <summary>
        /// OutStat -> write ( WriteList ) | writeln ( WriteList )
        /// </summary>
        private void OutStat()
        {
            if (Token == Tokens.WriteT)
            {
                Match(Tokens.WriteT);
                Match(Tokens.LParenT);
                WriteList();
                Match(Tokens.RParenT);
            }
            else if (Token == Tokens.WritelnT)
            {
                Match(Tokens.WritelnT);
                Match(Tokens.LParenT);
                WriteList();
                tacGenerator.GenerateLineOfTAC($"wrln");
                Match(Tokens.RParenT);
            }
        }

        /// <summary>
        /// WriteList -> WriteToken WriteListTail
        /// </summary>
        private void WriteList()
        {
            WriteToken();
            WriteListTail();
        }

        /// <summary>
        /// WriteListTail -> , WriteToken WriteListTail | ε
        /// </summary>
        private void WriteListTail()
        {
            if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                WriteToken();
                WriteListTail();
            }
        }

        /// <summary>
        /// WriteToken -> IdT | NumT | QuoteT
        /// </summary>
        private void WriteToken()
        {
            if (Token == Tokens.IdT)
            {
                ITableEntry entry = symbolTable.Lookup(Lexeme);

                if (entry != null && entry.typeOfEntry == EntryType.varEntry)
                {
                    tacGenerator.GenerateLineOfTAC($"wri {entry.bpOffsetName}");
                }
                else if (entry != null)
                {
                    ErrorHandler.LogError($"only set up to handle integer variables");
                }
                else
                {
                    ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
                }

                Match(Tokens.IdT);
            }
            else if (Token == Tokens.NumT)
            {
                tacGenerator.GenerateLineOfTAC($"wri {Lexeme}");
                Match(Tokens.NumT);
            }
            else if (Token == Tokens.LiteralT)
            {
                tacGenerator.GenerateLineOfTAC($"wrs S{AssemblyFile.literalNum}");
                AssemblyFile.AddLiteral(Lexeme);
                Match(Tokens.LiteralT);
            }
        }
    }
}