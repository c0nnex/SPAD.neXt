
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Comparators
{
    public class ComparatorEqual : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) == 0;
        }
    }
    public class ComparatorUnequal : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) != 0;
        }
    }
}
