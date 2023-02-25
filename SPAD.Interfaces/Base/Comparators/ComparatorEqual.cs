
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

    public class ComparatorStrEqual : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return Convert.ToString(testVal, CultureInfo.InvariantCulture) == (Convert.ToString(compareValueLeft, CultureInfo.InvariantCulture));
        }
    }
    public class ComparatorStrUnequal : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return Convert.ToString(testVal, CultureInfo.InvariantCulture) != (Convert.ToString(compareValueLeft, CultureInfo.InvariantCulture));
        }
    }

    public class ComparatorStrContains : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return Convert.ToString(testVal, CultureInfo.InvariantCulture).Contains(Convert.ToString(compareValueLeft, CultureInfo.InvariantCulture));
        }
    }

    public class ComparatorStrNotContains : ComparatorBase
    {
        protected override bool DoesMatchImpl(IComparable testVal)
        {
            return !Convert.ToString(testVal, CultureInfo.InvariantCulture).Contains(Convert.ToString(compareValueLeft, CultureInfo.InvariantCulture));
        }
    }

}
