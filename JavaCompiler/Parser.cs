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
            Match(Tokens.IdT);
            Match(Tokens.LBraceT);
            Match(Tokens.PublicT);
            Match(Tokens.StaticT);
            Match(Tokens.VoidT);
            Match(Tokens.MainT);
            Match(Tokens.LParenT);
            Match(Tokens.StringT);
            Match(Tokens.LBrackT);
            Match(Tokens.RBrackT);
            Match(Tokens.IdT);
            Match(Tokens.RParenT);
            Match(Tokens.LBraceT);
            SeqOfStatements();
            Match(Tokens.RBraceT);
            Match(Tokens.RBraceT);
        }

        /// <summary>
        /// ClassDecl -> ClassT IdT { VarDecl MethodDecl } |
        ///              ClassT IdT ExtendsT IdT { VarDecl MethodDecl } |
        ///              ε
        /// </summary>
        private void ClassDecl()
        {
            Match(Tokens.ClassT);
            Match(Tokens.IdT);

            if (Token == Tokens.ExtendsT)
            {
                Match(Tokens.ExtendsT);
                Match(Tokens.IdT);
            }

            Match(Tokens.LBraceT);
            VarDecl();
            MethodDecl();
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

                TableEntry entry = new TableEntry(Lexeme, Token, Depth);
                symbolTable.InsertEntry(entry);

                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);

                symbolTable.ConvertEntryToConstant(entry);

                Match(Tokens.NumT);
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
            else if (Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected type declaration, found \"{Lexeme}\"");
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
                Size = 2;
                Match(Tokens.IntT);
            }
            else if (Token == Tokens.FloatT)
            {
                TypeVar = VarType.floatType;
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
                TableEntry entry = new TableEntry(Lexeme, Token, Depth);
                symbolTable.InsertEntry(entry);
                symbolTable.ConvertEntryToVariable(entry);

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

                TableEntry entry = new TableEntry(Lexeme, Token, Depth);
                symbolTable.InsertEntry(entry);
                symbolTable.ConvertEntryToVariable(entry);

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
                Match(Tokens.IdT);
                Match(Tokens.LParenT);
                FormalList();
                Match(Tokens.RParenT);
                Match(Tokens.LBraceT);
                VarDecl();
                SeqOfStatements();
                Match(Tokens.ReturnT);
                Expr();
                Match(Tokens.SemiT);
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
                Match(Tokens.IdT);
                FormalRest();
            }
            else if (Types.Contains(Token) || Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected \",\", found \"{Lexeme}\"");
            }
        }

        /// <summary>
        /// ε
        /// </summary>
        private void SeqOfStatements()
        {
            // not yet implemented
        }

        /// <summary>
        /// ε
        /// </summary>
        private void Expr()
        {
            // not yet implemented
        }
    }
}