using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace System
{
    public static class SPADSystemExtensions
    {
        public static string Right(this string inStr, int numChars)
        {
            if (String.IsNullOrEmpty(inStr) || (inStr.Length < numChars) || (numChars <= 0))
                return inStr;
            return inStr.Substring(inStr.Length - numChars, numChars);
        }

        public static string Left(this string inStr, int numChars)
        {
            if (String.IsNullOrEmpty(inStr) || (inStr.Length < numChars) || (numChars <= 0))
                return inStr;
            return inStr.Substring(0, numChars);
        }
    }
}

namespace System.Collections.ObjectModel
{
    
    public class ObservableCollectionAsync<T> : ObservableCollection<T>
    {
        private readonly object _lockObject = new object();

        public ObservableCollectionAsync() : base()
        {
            BindingOperations.EnableCollectionSynchronization(this, _lockObject);
        }
        public ObservableCollectionAsync(List<T> list) : base(list)
        {
            BindingOperations.EnableCollectionSynchronization(this, _lockObject);
        }
        public ObservableCollectionAsync(IEnumerable<T> collection) : base(collection)
        {
            BindingOperations.EnableCollectionSynchronization(this, _lockObject);
        }
    }
}

namespace SPAD.neXt.Interfaces
{
    public delegate void EventHandler<TEventType, TEventArgs>(TEventType sender, TEventArgs e);

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
        public static Decimal Rescale(this Decimal value, Decimal sourceMin, Decimal sourceMax, Decimal targetMin, Decimal targetMax)
        {
            value = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return ((value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Rescale(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            value = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return ((value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
        }
    }

    public static class DictionaryExtensions
    {
        public delegate bool Predicate<TKey, TValue>(KeyValuePair<TKey, TValue> d);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveWhere<TKey, TValue>(
            this Dictionary<TKey, TValue> hashtable, Predicate<TKey, TValue> p)
        {
            foreach (KeyValuePair<TKey, TValue> value in hashtable.ToList().Where(value => p(value)))
                hashtable.Remove(value.Key);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveWhere<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> hashtable, Predicate<TKey, TValue> p)
        {
            foreach (KeyValuePair<TKey, TValue> value in hashtable.ToList().Where(value => p(value)))
                hashtable.Remove(value.Key);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> hashtable, TKey key) where TValue : class
        {
            TValue valOut;
            if (hashtable.TryGetValue(key, out valOut))
                return valOut;
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> hashtable, TKey key, TValue value) where TValue : class
        {
            TValue valOut;
            if (hashtable.TryGetValue(key, out valOut))
                return valOut;
            hashtable.Add(key, value);
            return value;
        }

        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            int tries = 0;
            TValue value;
            if ((key == null) || !dict.ContainsKey(key))
                return false;
            while (!dict.TryRemove(key, out value))
            {
                tries++;
                if (tries > 10)
                {
                    //Global.Logger.Warn("ConCurrent remove for {0} failed after 10 tries", key);
                    return false;
                }
                Thread.Sleep(100);
            }
            return true;
        }

        public static bool Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (key == null)
                return false;
            if (dict.ContainsKey(key))
                dict.Remove(key);
            int tries = 0;
            while (!dict.TryAdd(key, value))
            {
                tries++;
                if (tries > 10)
                {
                    //Global.Logger.Warn("ConCurrent add for {0} failed after 10 tries", key);
                    return false;
                }
                Thread.Sleep(100);
            }
            return true;
        }
    }

    public interface INotifyPropertyChangedHandler : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }
}

