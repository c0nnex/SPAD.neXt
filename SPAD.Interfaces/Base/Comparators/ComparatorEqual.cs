
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Comparators
{
    public class ComparatorEqual : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) == 0;
        }
    }
    public class ComparatorUnequal : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) != 0;
        }
    }

    public class ComparatorStrContains : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return Convert.ToString(compareValueLeft,CultureInfo.InvariantCulture).Contains(Convert.ToString(testVal,CultureInfo.InvariantCulture));
        }
    }

    public class ComparatorStrNotContains : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return !Convert.ToString(compareValueLeft, CultureInfo.InvariantCulture).Contains(Convert.ToString(testVal, CultureInfo.InvariantCulture));
        }
    }

}
