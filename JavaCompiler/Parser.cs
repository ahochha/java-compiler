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
            //MoreClasses();
            MainClass();
        }

        public void MoreClasses()
        {
            //not implemented yet...
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

        }

        public void VarDecl()
        {

        }

        public void IdentifierList()
        {

        }

        public void Type()
        {

        }

        public void MethodDecl()
        {

        }

        public void FormalList()
        {

        }

        public void FormalRest()
        {

        }

        public void SeqOfStatements()
        {
            // empty?
        }

        public void Expr()
        {

        }
    }
}
