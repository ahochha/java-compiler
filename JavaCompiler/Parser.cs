using static JavaCompiler.Resources;

namespace JavaCompiler
{
    public class Parser
    {
        private LexicalAnalyzer lexicalAnalyzer { get; set; }

        public Parser()
        {
            lexicalAnalyzer = new LexicalAnalyzer();
            lexicalAnalyzer.GetNextToken();
        }

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

        public void Prog()
        {
            MoreClasses();
            MainClass();
        }

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

        private void VarDecl()
        {
            if (Token == Tokens.FinalT)
            {
                Match(Tokens.FinalT);
                Type();
                Match(Tokens.IdT);
                Match(Tokens.AssignOpT);
                Match(Tokens.NumT);
                Match(Tokens.SemiT);
                VarDecl();
            }
            else if (Types.Contains(Token))
            {
                Type();

                if (Token == Tokens.SemiT)
                {
                    ErrorHandler.LogError($"expected an identifier, found \"{Lexeme}\"");
                }

                IdentifierList();
                Match(Tokens.SemiT);
                VarDecl();
            }
            else if (Token == Tokens.IdT)
            {
                ErrorHandler.LogError($"expected type declaration, found \"{Lexeme}\"");
            }
        }

        private void Type()
        {
            if (Types.Contains(Token))
            {
                Match(Types.Find(t => t == Token));
            }
            else
            {
                ErrorHandler.LogError($"expected type declaration, found \"{Lexeme}\"");
            }
        }

        private void IdentifierList()
        {
            if (Token == Tokens.IdT)
            {
                Match(Tokens.IdT);
                IdentifierList();
            }
            else if (Token == Tokens.CommaT)
            {
                Match(Tokens.CommaT);
                Match(Tokens.IdT);

                if (Token == Tokens.IdT)
                {
                    ErrorHandler.LogError("expected \",\" before next identifier");
                }

                IdentifierList();
            }
        }

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

        private void SeqOfStatements()
        {
            // not yet implemented
        }

        private void Expr()
        {
            // not yet implemented
        }
    }
}
