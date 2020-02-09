using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public class Parser : Resources
    {
        public Scanner scanner { get; set; }

        public Parser(FileHandler file)
        {
            scanner = new Scanner(file);
            scanner.GetNextToken();
        }

        public void match(Symbol desired)
        {
            if (Token == desired)
            {
                scanner.GetNextToken();
            }
            else
            {
                Console.WriteLine("Parse Error");
            }
        }

        public void Prog()
        {
            MoreClasses();
            MainClass();
        }

        public void MoreClasses()
        {
            if (Token == Symbol.ClassT)
            {
                ClassDecl();
                MoreClasses();
            }
        }

        public void MainClass()
        {
            match(Symbol.FinalT);
            match(Symbol.ClassT);
            match(Symbol.IdT);
            match(Symbol.LBraceT);
            match(Symbol.PublicT);
            match(Symbol.StaticT);
            match(Symbol.VoidT);
            match(Symbol.MainT);
            match(Symbol.LParenT);
            match(Symbol.StringT);
            match(Symbol.LBrackT);
            match(Symbol.RBrackT);
            match(Symbol.IdT);
            match(Symbol.RParenT);
            match(Symbol.LBraceT);
            SeqOfStatements();
            match(Symbol.RBraceT);
            match(Symbol.RBraceT);
        }

        public void ClassDecl()
        {
            match(Symbol.ClassT);
            match(Symbol.IdT);

            if (Token == Symbol.ExtendsT)
            {
                match(Symbol.ExtendsT);
                match(Symbol.IdT);
            }

            match(Symbol.LBraceT);
            VarDecl();
            MethodDecl();
            match(Symbol.RBraceT);
        }

        public void VarDecl()
        {
            if (Token == Symbol.FinalT)
            {
                match(Symbol.FinalT);
                Type();
                match(Symbol.IdT);
                match(Symbol.AssignOpT);
                match(Symbol.NumT);
                match(Symbol.SemiT);
                VarDecl();

            }
            else if (Types.Contains(Token))
            {
                Type();
                IdentifierList();
                match(Symbol.SemiT);
                VarDecl();
            }
        }

        public void Type()
        {
            if (Token == Symbol.IntT)
            {
                match(Symbol.IntT);
            }
            else if (Token == Symbol.BooleanT)
            {
                match(Symbol.BooleanT);
            }
            else if (Token == Symbol.VoidT)
            {
                match(Symbol.VoidT);
            }
        }

        public void IdentifierList()
        {
            if (Token == Symbol.IdT)
            {
                match(Symbol.IdT);
                IdentifierList();
            }
            else if (Token == Symbol.CommaT)
            {
                match(Symbol.CommaT);
                match(Symbol.IdT);
            }
        }

        public void MethodDecl()
        {
            if (Token == Symbol.PublicT)
            {
                match(Symbol.PublicT);
                Type();
                match(Symbol.IdT);
                match(Symbol.LParenT);
                FormalList();
                match(Symbol.RParenT);
                match(Symbol.LBraceT);
                VarDecl();
                SeqOfStatements();
                match(Symbol.ReturnT);
                Expr();
                match(Symbol.SemiT);
                match(Symbol.RBraceT);
                MethodDecl();
            }
        }

        public void FormalList()
        {
            if (Types.Contains(Token))
            {
                Type();
                match(Symbol.IdT);
                FormalRest();
                FormalList();
            }
        }

        public void FormalRest()
        {
            if (Token == Symbol.CommaT)
            {
                match(Symbol.CommaT);
                Type();
                match(Symbol.IdT);
                FormalRest();
            }
        }

        public void SeqOfStatements()
        {
            // not yet implemented
        }

        public void Expr()
        {
            // not yet implemented
        }
    }
}
