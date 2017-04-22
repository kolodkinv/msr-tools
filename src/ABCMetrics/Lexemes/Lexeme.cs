using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCMetrics
{
    public class Lexeme
    {
        private string name;
        private TypeLexeme type;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public TypeLexeme Type
        {
            get { return type; }
            set { type = value; }
        }

        public Lexeme()
        {
            name = "";
            type = TypeLexeme.Other;
        }
        
    }

    
}
