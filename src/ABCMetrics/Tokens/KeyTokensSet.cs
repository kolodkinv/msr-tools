using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ABCMetrics
{
    public abstract class KeyTokensSet
    {
        public abstract ReadOnlyCollection<string> Assignment
        {
            get;
        }
        public abstract ReadOnlyCollection<string> Branch
        {
            get;
        }

        public abstract ReadOnlyCollection<string> Condition
        {
            get;
        }

        public abstract ReadOnlyCollection<string> AdditionToken
        {
            get;
        }

    }
}
