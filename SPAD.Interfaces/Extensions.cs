using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public static class SPADExtensions
    {
        #region Enums

        public static T SafeParse<T>(this Enum value, string valIn)
        {
            try
            {
                return  (T)Enum.Parse(value.GetType(), valIn,true);
            }
            catch { return default(T); }
        }

        public static T SetFlag<T>(this Enum value, T flag)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());

            // note: AsInt mean: math integer vs enum (not the c# int type)
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
           
            valueAsInt |= flagAsInt;
           
            return (T)valueAsInt;
        }

        public static T HasFlag<T>(this Enum value, T flag)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());

            // note: AsInt mean: math integer vs enum (not the c# int type)
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);

            return ( valueAsInt & flagAsInt ) == valueAsInt;

        }

        public static T ClearFlag<T>(this Enum value, T flag)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());

            // note: AsInt mean: math integer vs enum (not the c# int type)
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
            
            valueAsInt &= ~flagAsInt;
            
            return (T)valueAsInt;
        }

        public static int GetSetBitCount(this long lValue)
        {
            int iCount = 0;

            //Loop the value while there are still bits
            while (lValue != 0)
            {
                //Remove the end bit
                lValue = lValue & (lValue - 1);

                //Increment the count
                iCount++;
            }

            //Return the count
            return iCount;
        }

        public static IEnumerable<T> GetFlags<T>(this T flags)  
        {
            foreach (T x in Enum.GetValues(typeof(T)))
            {
                dynamic q = flags;
                if (q.HasFlags(x))
                    yield return x;
            }
        }

        public static int CountFlags<T>(this T value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());

            // note: AsInt mean: math integer vs enum (not the c# int type)
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            int iCount = 0;

            //Loop the value while there are still bits
            while (valueAsInt != 0)
            {
                //Remove the end bit
                valueAsInt = valueAsInt & (valueAsInt - 1);

                //Increment the count
                iCount++;
            }
            //Return the count
            return iCount;
        }
        #endregion

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
                // do nothing
            }
        }
    }

    public static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(this int value, int minimum, int maximum)
        {
            if (maximum < minimum)
            {
                minimum = 0;
                maximum = ushort.MaxValue;
            }

            var midpoint = minimum + ((minimum + maximum) / 2);

            value = Math.Max(minimum, Math.Min(value, maximum));

            if (Math.Abs(midpoint - value) > 1)
            {
                return (value - minimum) / (float)(maximum - minimum);
            }
            else
            {
                return 0.5f;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Rescale(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            value = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return ((value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
        }
    }
}
