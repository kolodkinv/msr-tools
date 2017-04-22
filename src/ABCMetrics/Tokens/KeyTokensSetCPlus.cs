using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCMetrics 
{
    public class KeyTokensSetCPlus : KeyTokensSet
    {
        private ReadOnlyCollection<string> assignment =
            new ReadOnlyCollection<string>(
                new List<string> { "*=", "/=", "%=", "+=", "<<=", ">>=", "&=", "|=", "^=", "++", "--", "=" });

        private ReadOnlyCollection<string> branch =
            new ReadOnlyCollection<string>(
                new List<string> { "goto", "return", "break", "for", "new", "delete" });

        private ReadOnlyCollection<string> condition =
            new ReadOnlyCollection<string>(
                new List<string> { "if", "else", "default", "?", "try", "catch" , "&&" , "||" , "case" , "while"});

        private ReadOnlyCollection<string> additionToken =
            new ReadOnlyCollection<string>(
                new List<string> { ";", "\r", "\n", "->", "==", "!=", "<=", ">=", ",", "switch" });

        public override ReadOnlyCollection<string> Assignment
        {
            get { return assignment; }
        }

        public override ReadOnlyCollection<string> Branch
        {
            get { return branch; }
        }

        public override ReadOnlyCollection<string> Condition
        {
            get { return condition; }
        }

        public override ReadOnlyCollection<string> AdditionToken
        {
            get { return additionToken; }
        }
    }
}
