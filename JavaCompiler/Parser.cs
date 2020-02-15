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

        private void Match(Symbol desired)
        {
            if (Token == desired)
            {
                lexicalAnalyzer.GetNextToken();
            }
            else
            {
                ErrorHandler.LogError(JavaFile.lineNum, desired);
            }
        }

        public void Prog()
        {
            MoreClasses();
            MainClass();
        }

        private void MoreClasses()
        {
            if (Token == Symbol.ClassT)
            {
                ClassDecl();
                MoreClasses();
            }
        }

        private void MainClass()
        {
            Match(Symbol.FinalT);
            Match(Symbol.ClassT);
            Match(Symbol.IdT);
            Match(Symbol.LBraceT);
            Match(Symbol.PublicT);
            Match(Symbol.StaticT);
            Match(Symbol.VoidT);
            Match(Symbol.MainT);
            Match(Symbol.LParenT);
            Match(Symbol.StringT);
            Match(Symbol.LBrackT);
            Match(Symbol.RBrackT);
            Match(Symbol.IdT);
            Match(Symbol.RParenT);
            Match(Symbol.LBraceT);
            SeqOfStatements();
            Match(Symbol.RBraceT);
            Match(Symbol.RBraceT);
        }

        private void ClassDecl()
        {
            if (Token == Symbol.ClassT)
            {
                Match(Symbol.ClassT);
                Match(Symbol.IdT);

                if (Token == Symbol.ExtendsT)
                {
                    Match(Symbol.ExtendsT);
                    Match(Symbol.IdT);
                }

                Match(Symbol.LBraceT);
                VarDecl();
                MethodDecl();
                Match(Symbol.RBraceT);
            }
        }

        private void VarDecl()
        {
            if (Token == Symbol.FinalT)
            {
                Match(Symbol.FinalT);
                Type();
                Match(Symbol.IdT);
                Match(Symbol.AssignOpT);
                Match(Symbol.NumT);
                Match(Symbol.SemiT);
                VarDecl();
            }
            else if (Types.Contains(Token))
            {
                Type();
                IdentifierList();
                Match(Symbol.SemiT);
                VarDecl();
            }
        }

        private void Type()
        {
            Match(Types.Find(t => t == Token));
        }

        private void IdentifierList()
        {
            if (Token == Symbol.IdT)
            {
                Match(Symbol.IdT);
                IdentifierList();
            }
            else if (Token == Symbol.CommaT)
            {
                Match(Symbol.CommaT);
                Match(Symbol.IdT);
                IdentifierList();
            }
        }

        private void MethodDecl()
        {
            if (Token == Symbol.PublicT)
            {
                Match(Symbol.PublicT);
                Type();
                Match(Symbol.IdT);
                Match(Symbol.LParenT);
                FormalList();
                Match(Symbol.RParenT);
                Match(Symbol.LBraceT);
                VarDecl();
                SeqOfStatements();
                Match(Symbol.ReturnT);
                Expr();
                Match(Symbol.SemiT);
                Match(Symbol.RBraceT);
                MethodDecl();
            }
        }

        private void FormalList()
        {
            if (Types.Contains(Token))
            {
                Type();
                Match(Symbol.IdT);
                FormalRest();
                FormalList();
            }
        }

        private void FormalRest()
        {
            if (Token == Symbol.CommaT)
            {
                Match(Symbol.CommaT);
                Type();
                Match(Symbol.IdT);
                FormalRest();
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
