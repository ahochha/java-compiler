using System.Collections.Generic;
using System.Linq;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class Parser
    {
        private LexicalAnalyzer lexicalAnalyzer { get; set; }
        public SymbolTable symbolTable { get; set; }
        public TACTranslator tacTranslator { get; set; }

        public Parser()
        {
            lexicalAnalyzer = new LexicalAnalyzer();
            symbolTable = new SymbolTable();
            tacTranslator = new TACTranslator();

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
            if (Token == Tokens.PublicT)
            {
                Match(Tokens.PublicT);
                Type();
                TypeReturn = TypeVar;
                TableEntry entry = symbolTable.CreateTableEntry();
                MethodNames.Add(Lexeme);
                tacTranslator.GenerateLineOfTAC($"Proc {entry.lexeme}");
                Match(Tokens.IdT);
                Depth++;
                ResetMethodGlobals();
                Match(Tokens.LParenT);
                ParameterVarsSize = 4;
                FormalList();
                Offset = 2;
                tacTranslator.tempVarOffset = 2;
                Match(Tokens.RParenT);
                Match(Tokens.LBraceT);
                VarDecl();
                SeqOfStatements();
                Match(Tokens.ReturnT);
                Expr();
                Match(Tokens.SemiT);
                symbolTable.ConvertEntryToMethod(entry);
                symbolTable.DeleteDepth(Depth);
                tacTranslator.GenerateLineOfTAC($"Endp {entry.lexeme}");
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
            if (Token == Tokens.IdT)
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
            AssignStat();
            //IOStat(); - not yet implemented...
        }

        /// <summary>
        /// AssignStat -> IdT = Expr | IdT = MethodCall | MethodCall
        /// </summary>
        private void AssignStat()
        {
            ITableEntry firstIdEntry = symbolTable.Lookup(Lexeme);

            if (firstIdEntry != null && firstIdEntry.typeOfEntry == EntryType.classEntry)
            {
                MethodCall(firstIdEntry as Class);
            }
            else if (firstIdEntry != null && firstIdEntry.typeOfEntry == EntryType.varEntry)
            {
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);

                ITableEntry secondIdEntry = symbolTable.Lookup(Lexeme);

                if (secondIdEntry != null && secondIdEntry.typeOfEntry == EntryType.classEntry)
                {
                    MethodCall(secondIdEntry as Class);
                    tacTranslator.GenerateLineOfTAC($"{firstIdEntry.lexeme} = _ax");
                }
                else 
                {
                    Expr();
                    tacTranslator.GenerateFinalExpressionTAC(firstIdEntry as Variable);
                }
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }
        }

        /// <summary>
        /// ε
        /// </summary>
        private void IOStat()
        {
            // not yet implemented...
        }

        /// <summary>
        /// Expr -> Relation | ε
        /// </summary>
        private void Expr()
        {
            if (FactorTokens.Contains(Token))
            {
                Relation();
                tacTranslator.Eplace = tacTranslator.Tplace;
            }
        }

        /// <summary>
        /// Relation -> SimpleExpr
        /// </summary>
        private void Relation()
        {
            SimpleExpr();
        }

        /// <summary>
        /// SimpleExpr -> Term MoreTerm
        /// </summary>
        private void SimpleExpr()
        {
            Term();
            tacTranslator.Rplace = tacTranslator.Tplace;
            MoreTerm();

            if (tacTranslator.tempVarStack.Count > 0)
            {
                tacTranslator.Tplace.bpOffsetVarName = tacTranslator.tempVarStack.Pop();
            }
        }

        /// <summary>
        /// MoreTerm -> AddOpT Term MoreTerm | ε
        /// </summary>
        private void MoreTerm()
        {
            if (Token == Tokens.AddOpT)
            {
                tacTranslator.hasSegments = true;
                tacTranslator.NewTempVar();
                tacTranslator.tempVarStack.Push(tacTranslator.tempVarName);
                tacTranslator.GenerateSegmentOfExpressionTAC();
                AddOp();
                tacTranslator.bpStack.Push(tacTranslator.code);
                Term();
                tacTranslator.code = tacTranslator.bpStack.Pop();
                tacTranslator.code += tacTranslator.Tplace.bpOffsetVarName;
                tacTranslator.Rplace.bpOffsetVarName = tacTranslator.tempVarName;
                tacTranslator.GenerateLineOfTAC(tacTranslator.code);
                MoreTerm();
            }
        }

        /// <summary>
        /// Term -> Factor MoreFactor
        /// </summary>
        private void Term()
        {
            Factor();
            tacTranslator.Rplace = tacTranslator.Tplace;
            MoreFactor();

            if (tacTranslator.tempVarStack.Count > 0)
            {
                tacTranslator.Tplace.bpOffsetVarName = tacTranslator.tempVarStack.Pop();
            }
        }
           

        /// <summary>
        /// MoreFactor -> MulOpT Factor MoreFactor | ε
        /// </summary>
        private void MoreFactor()
        {
            if (Token == Tokens.MulOpT)
            {
                tacTranslator.hasSegments = true;
                tacTranslator.NewTempVar();
                tacTranslator.tempVarStack.Push(tacTranslator.tempVarName);
                tacTranslator.GenerateSegmentOfExpressionTAC();
                MulOp();
                tacTranslator.bpStack.Push(tacTranslator.code);
                Factor();
                tacTranslator.code = tacTranslator.bpStack.Pop();
                tacTranslator.code += tacTranslator.Tplace.bpOffsetVarName;
                tacTranslator.Rplace.bpOffsetVarName = tacTranslator.tempVarName;
                tacTranslator.GenerateLineOfTAC(tacTranslator.code);
                MoreFactor();
            }
        }

        /// <summary>
        /// Factor - > IdT | NumT | (Expr) | ! Factor | SignOp Factor | TrueT | FalseT
        /// </summary>
        private void Factor()
        {
            if (Token == Tokens.IdT)
            {
                tacTranslator.Tplace = new Variable(symbolTable.Lookup(Lexeme) as Variable);

                if (tacTranslator.Tplace != null)
                {
                    //tacTranslator.Tplace.bpOffsetVarName = $"_bp-{tacTranslator.Tplace.offset}";
                    Match(Tokens.IdT);
                }
                else
                {
                    ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
                }
            }
            else if (Token == Tokens.NumT)
            {
                tacTranslator.Tplace = new Variable();
                tacTranslator.tempVarName = Lexeme;
                tacTranslator.Tplace.lexeme = Lexeme;
                Match(Tokens.NumT);
            }
            else if (Token == Tokens.LParenT)
            {
                Match(Tokens.LParenT);
                //tacTranslator.exprStack.Push(tacTranslator.tempVarStack);
               // tacTranslator.tempVarStack = new Stack<string>();
                Expr();
                //tacTranslator.tempVarStack = tacTranslator.exprStack.Pop();
                Match(Tokens.RParenT);
            }
            else if (Token == Tokens.NotOpT)
            {
                Match(Tokens.NotOpT);
                Factor();
            }
            else if (Token == Tokens.AddOpT && Lexeme == "-")
            {
                SignOp();
                Factor();
            }
            else if (Token == Tokens.TrueT)
            {
                Match(Tokens.TrueT);
            }
            else if (Token == Tokens.FalseT)
            {
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

            if (classEntry.methodNames.Contains(Lexeme) || MethodNames.Contains(Lexeme))
            {
                ITableEntry entry = symbolTable.Lookup(Lexeme);

                if (entry != null)
                {
                    Match(Tokens.IdT);
                    Match(Tokens.LParenT);
                    Params();
                    Match(Tokens.RParenT);
                    tacTranslator.GenerateLineOfTAC($"Call {entry.lexeme}");
                }
                else
                {
                    ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
                }
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
            else if (Token == Tokens.NumT)
            {
                tacTranslator.GenerateLineOfTAC($"push {Lexeme}");
                Match(Tokens.NumT);
                ParamsTail();
            }
            else if (Token != Tokens.RParenT)
            {
                ErrorHandler.LogError($"parameters to method calls must be variables or numbers");
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
                else if (Token == Tokens.NumT)
                {
                    tacTranslator.GenerateLineOfTAC($"push {Lexeme}");
                    Match(Tokens.NumT);
                    ParamsTail();
                }
                else
                {
                    ErrorHandler.LogError($"parameters to method calls must be variables or numbers");
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
                tacTranslator.GenerateLineOfTAC($"push {var.bpOffsetVarName}");
                Match(Tokens.IdT);
                ParamsTail();
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }
        }
    }
}