using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Comparators
{
    public delegate IComparable ComparatorGetValueDelegate();

    public abstract class ComparatorBase : ISPADComparator
    {
        public static ComparatorBase IgnoreComparator = new ComparatorIgnore();
        public IComparable compareValueLeft { get; private set; } = 0;
        public IComparable compareValueRight { get; private set; } = 0;
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
                case SPADEventValueComparator.StrEquals:
                    b = new ComparatorEqual(); break;

                case SPADEventValueComparator.Unequal:
                case SPADEventValueComparator.StrNotEquals:
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
                case SPADEventValueComparator.IsBitSet:
                    b = new ComparatorIsBitSet(); break;
                case SPADEventValueComparator.IsBitNotSet:
                    b = new ComparatorIsBitNotSet(); break;
                case SPADEventValueComparator.Always:
                    break;
                case SPADEventValueComparator.StrContains:
                    b = new ComparatorStrContains(); break;                    
                case SPADEventValueComparator.StrNotContains:
                    b = new ComparatorStrNotContains(); break;
                case SPADEventValueComparator.None:
                    break;
                default:
                    b = new ComparatorAlways(); break;
            }
            return b;
        }

        public static bool IsStringComparator(SPADEventValueComparator whichComparator)
        {
            switch (whichComparator)
            {
                case SPADEventValueComparator.Equals:
                case SPADEventValueComparator.Unequal:
                case SPADEventValueComparator.Less:
                case SPADEventValueComparator.LessOrEqual:
                case SPADEventValueComparator.Greater:
                case SPADEventValueComparator.GreaterOrEqual:
                case SPADEventValueComparator.Mask:
                case SPADEventValueComparator.Not:
                case SPADEventValueComparator.AnyBitSet:
                case SPADEventValueComparator.IsBitSet:
                case SPADEventValueComparator.IsBitNotSet:
                case SPADEventValueComparator.Ignore:
                case SPADEventValueComparator.Always:
                case SPADEventValueComparator.Range:
                case SPADEventValueComparator.None:
                    return false;
                case SPADEventValueComparator.StrEquals:
                case SPADEventValueComparator.StrNotEquals:
                case SPADEventValueComparator.StrContains:
                case SPADEventValueComparator.StrNotContains:
                    return true;
                default:
                    return false;
            }
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

        protected abstract bool DoesMatchImpl(IComparable testValue);

        public bool DoesMatch(IComparable testValue)
        {
            try
            {
                if (testValue == null)
                    return false;
                if (compareValueLeftDelegate != null)
                    compareValueLeft = compareValueLeftDelegate();
                if (compareValueRightDelegate != null)
                    compareValueRight = compareValueRightDelegate();
                return DoesMatchImpl(testValue);
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return $"[Comperator {GetType().Name}: {compareValueLeft} {compareValueRight}]";
        }
    }
}
