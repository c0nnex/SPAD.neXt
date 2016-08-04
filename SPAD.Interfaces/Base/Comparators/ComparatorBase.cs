using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Comparators
{
    public delegate double ComparatorGetValueDelegate();

    public abstract class ComparatorBase : ISPADComparator
    {
        public static ComparatorBase IgnoreComparator = new ComparatorIgnore();
        public Double compareValueLeft { get; private set; } = 0;
        public Double compareValueRight { get; private set; } = 0;
        private ComparatorGetValueDelegate compareValueLeftDelegate = null;
        private ComparatorGetValueDelegate compareValueRightDelegate = null;

        public ComparatorBase()
        {
            compareValueLeft = compareValueRight = 0;
        }

        public ComparatorBase(object cmpValue)
        {
            compareValueLeft = Convert.ToDouble(cmpValue);
        }

        private static ComparatorBase CreateComparator(SPADEventValueComparator whichComparator)
        {
            ComparatorBase b = null;
            switch (whichComparator)
            {
                case SPADEventValueComparator.Equals:
                    b = new ComparatorEqual(); break;

                case SPADEventValueComparator.Unequal:
                    b = new ComparatorUnequal(); break;

                case SPADEventValueComparator.Less:
                    b = new ComparatorLess(); break;

                case SPADEventValueComparator.LessOrEqual:
                    b = new ComparatorLessOrEqual(); break;

                case SPADEventValueComparator.Greater:
                    b = new ComparatorGreater(); break;

                case SPADEventValueComparator.GreaterOrEqual:
                    b = new ComparatorGreaterOrEqual(); break;

                case SPADEventValueComparator.Mask:
                    b = new ComparatorMask(); break;

                case SPADEventValueComparator.Not:
                    b = new ComparatorNot(); break;

                case SPADEventValueComparator.AnyBitSet:
                    b = new ComparatorAnyBitSet(); break;

                case SPADEventValueComparator.Ignore:
                    return IgnoreComparator;
                case SPADEventValueComparator.Range:
                    b = new ComparatorRange(); break;
                default:
                    b = new ComparatorAlways(); break;
            }
            return b;
        }

        public static ComparatorBase CreateComparator(SPADEventValueComparator whichComparator, object leftValue, object rightValue = null)
        {
            try
            {
                var b = CreateComparator(whichComparator);
                if (leftValue != null)
                    b.compareValueLeft = Convert.ToDouble(leftValue);
                if (rightValue != null)
                {
                    b.compareValueRight = Convert.ToDouble(rightValue);
                }
                return b;
            }
            catch { return IgnoreComparator; }
        }

        public static ComparatorBase CreateComparator(SPADEventValueComparator whichComparator, ComparatorGetValueDelegate leftDelegate, ComparatorGetValueDelegate rightDelegate = null)
        {
            try
            {
                var b = CreateComparator(whichComparator);
                b.compareValueLeftDelegate = leftDelegate;
                b.compareValueRightDelegate = rightDelegate;
                return b;
            }
            catch { return IgnoreComparator; }
        }

        protected abstract bool DoesMatch(IComparable testValue);

        public bool DoesMatch(object testValue)
        {
            try
            {
                if (testValue == null)
                    return false;
                if (compareValueLeftDelegate != null)
                    compareValueLeft = compareValueLeftDelegate();
                if (compareValueRightDelegate != null)
                    compareValueRight = compareValueRightDelegate();
                return DoesMatch(Convert.ToDouble(testValue) as IComparable);
            }
            catch
            {
                return false;
            }
        }
    }
}
