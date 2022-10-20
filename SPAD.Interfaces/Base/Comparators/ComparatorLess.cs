
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Comparators
{
    public class ComparatorLess : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testValue)
        {
            return compareValueLeft.CompareTo(testValue) > 0; // compareValueLeft > testValue -> TestValue < cLeft
        }
    }

    public class ComparatorLessOrEqual : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) >= 0;
        }
    }

    public class ComparatorGreater : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) < 0;
        }
    }

    public class ComparatorGreaterOrEqual : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            return compareValueLeft.CompareTo(testVal) <= 0;
        }
    }

    public class ComparatorRange : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testVal)
        {
            
            var Left = compareValueLeft.CompareTo(testVal);
            var Right = compareValueRight.CompareTo(testVal);

            if ( compareValueLeft.CompareTo(compareValueRight) >= 0)
            {
                return (Left >= 0) && (Right <= 0);
            }
            return (Left <= 0) && (Right >= 0);
        }
    }


    public class ComparatorAnyBitSet: ComparatorBase
    {               
        protected override bool DoesMatch(IComparable testValue)
        {           
            UInt64 a3 = Convert.ToUInt64(compareValueLeft);
            UInt64 a4 = Convert.ToUInt64(testValue);
            return (a3 & a4) != 0;
        }
    }

    public class ComparatorMask : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testValue)
        {
            UInt64 a3 = Convert.ToUInt64(compareValueLeft);
            UInt64 a4 = Convert.ToUInt64(testValue);
            return (a3 & a4) == a3;
        }
    }

    public class ComparatorIsBitSet : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testValue)
        {

            UInt64 a3 = 1UL << Convert.ToInt32(compareValueLeft);
            UInt64 a4 = Convert.ToUInt64(testValue);
            return (a3 & a4) == a3;
        }
    }
    public class ComparatorIsBitNotSet : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testValue)
        {
            UInt64 a3 = 1UL << Convert.ToInt32(compareValueLeft);
            UInt64 a4 = Convert.ToUInt64(testValue);
            return (a3 & a4) == a3;
        }
    }

    public class ComparatorNot : ComparatorBase
    {
        protected override bool DoesMatch(IComparable testValue)
        {
            UInt64 a3 = Convert.ToUInt64(compareValueLeft);
            UInt64 a4 = Convert.ToUInt64(testValue);
            return (a3 & a4) != a3;
        }
    }

    public class ComparatorIgnore : ComparatorBase
    {
        public ComparatorIgnore()            
        { }

        protected override bool DoesMatch(IComparable testValue)
        {
            return false;
        }
    }

    public class ComparatorAlways : ComparatorBase
    {
        public ComparatorAlways()            
        { }

        protected override bool DoesMatch(IComparable testValue)
        {
            return true;
        }
    }
    
}
