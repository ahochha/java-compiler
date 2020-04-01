using System.Collections.Generic;
using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class Parser
    {
        private LexicalAnalyzer lexicalAnalyzer { get; set; }
        public SymbolTable symbolTable { get; set; }

        public Parser()
        {
            lexicalAnalyzer = new LexicalAnalyzer();
            symbolTable = new SymbolTable();

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
            symbolTable.Display(Depth);
            symbolTable.DeleteDepth(Depth);
        }

        /// <summary>
        /// MoreClasses -> ClassDecl MoreClasses | ε
        /// </summary>
        private void MoreClasses()
        {
            if (Token == Tokens.ClassT)
            {
                ClassDecl();
                symbolTable.Display(Depth);
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
            symbolTable.Display(Depth);
            symbolTable.DeleteDepth(Depth);
            Depth--;
            Match(Tokens.RBraceT);
            symbolTable.ConvertEntryToClass(finalClassEntry);
            symbolTable.Display(Depth);
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
            symbolTable.Display(Depth);
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
                TypeConst = ConstType.intType;
                Size = 2;
                Match(Tokens.IntT);
            }
            else if (Token == Tokens.FloatT)
            {
                TypeVar = VarType.floatType;
                TypeConst = ConstType.floatType;
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
                Match(Tokens.IdT);
                Depth++;
                ResetMethodGlobals();
                Match(Tokens.LParenT);
                FormalList();
                Match(Tokens.RParenT);
                Match(Tokens.LBraceT);
                VarDecl();
                SeqOfStatements();
                Match(Tokens.ReturnT);
                Expr();
                Match(Tokens.SemiT);
                symbolTable.ConvertEntryToMethod(entry);
                symbolTable.Display(Depth);
                symbolTable.DeleteDepth(Depth);
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
                symbolTable.ConvertEntryToVariable(entry);
                ParameterTypes.Add(TypeVar);
                ParameterNum++;
                ParameterVarsSize += Size;
                Offset += Size;
                Match(Tokens.IdT);
                FormalRest();
                FormalList();
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

        private void Statement()
        {
            AssignStat();
            //IOStat(); - not yet implemented...
        }

        private void AssignStat()
        {
            ITableEntry entry = symbolTable.Lookup(Lexeme);

            if (entry != null && entry.depth == Depth)
            {
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);
                Expr();
            }
            else
            {
                ErrorHandler.LogError($"\"{Lexeme}\" is undeclared");
            }
        }

        private void IOStat()
        {
            // not yet implemented...
        }

        private void Expr()
        {
            if (FactorTokens.Contains(Token))
            {
                Relation();
            }
        }

        private void Relation()
        {
            SimpleExpr();
        }

        private void SimpleExpr()
        {
            Term();
            MoreTerm();
        }

        private void MoreTerm()
        {
            if (Token == Tokens.AddOpT)
            {
                AddOp();
                Term();
                MoreTerm();
            }
        }

        private void Term()
        {
            Factor();
            MoreFactor();
        }

        private void MoreFactor()
        {
            if (Token == Tokens.MulOpT)
            {
                MulOp();
                Factor();
                MoreFactor();
            }
        }

        private void Factor()
        {
            if (Token == Tokens.IdT)
            {
                Match(Tokens.IdT);
            }
            else if (Token == Tokens.NumT)
            {
                Match(Tokens.NumT);
            }
            else if (Token == Tokens.LParenT)
            {
                Match(Tokens.LParenT);
                Expr();
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

        private void AddOp()
        {
            Match(Tokens.AddOpT);
        }

        private void MulOp()
        {
            Match(Tokens.MulOpT);
        }

        private void SignOp()
        {
            Match(Tokens.AddOpT);
        }
    }
}