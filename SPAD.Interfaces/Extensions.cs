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
using System.Diagnostics;

namespace System
{
    public static class SPADSystemExtensions
    {
        public static string Right(this string inStr, int numChars)
        {
            if (String.IsNullOrEmpty(inStr) || (numChars <= 0))
                return String.Empty;
            return inStr.Substring(inStr.Length - Math.Min(numChars,inStr.Length), Math.Min(numChars, inStr.Length));
        }

        public static string Left(this string inStr, int numChars)
        {
            if (String.IsNullOrEmpty(inStr) || (numChars <= 0))
                return String.Empty;
            return inStr.Substring(0,Math.Min(numChars,inStr.Length));
        }

        public static string GetPart(this string s, int part, string splitter, string defaultVal = "")
        {
            var parts = s?.Split(new string[] { splitter }, StringSplitOptions.None);
            if (parts == null || part >= parts.Length)
                return defaultVal;
            return parts[part];
        }
        public static T GetPart<T>(this string s, int part, string splitter, T defaultVal = default)
        {
            var parts = s?.Split(new string[] { splitter }, StringSplitOptions.None);
            if (parts == null || part >= parts.Length)
                return defaultVal;
            try
            {
                object res;
                if (typeof(T) == typeof(Guid))
                {
                    if (Guid.TryParse(parts[part], out var resGuid))
                        res = resGuid;
                    else
                        res = Guid.Empty;
                    return (T)res;
                }
                if (typeof(T) == typeof(bool))
                {
                    res = parts[part] == "1" || String.Compare(parts[part], "true", true) == 0;
                    return (T)res;
                }
                res = Convert.ChangeType(parts[part], typeof(T));
                return (T)res;
            }
            catch { return defaultVal; }
        }

        public static string ReplacePart(this string s, int part, string splitter, string newVal)
        {
            var parts = s?.Split(new string[] { splitter }, StringSplitOptions.None);
            if (parts == null || part >= parts.Length)
                return s;
            parts[part] = newVal;
            return String.Join(splitter, parts);
        }

        public static string SkipParts(this string s, int startPart, int numParts, string splitter)
        {
            var parts = new List<string>(s?.Split(new string[] { splitter }, StringSplitOptions.None));
            while ((startPart < parts.Count) && (numParts > 0))
            {
                parts.RemoveAt(startPart);
                numParts--;
            }
            if (parts.Count == 0)
                return String.Empty;
            return String.Join(splitter, parts);
        }

        public static String[] Split(this string inStr, String separator)
        {
            return inStr.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string HexDump(this byte[] bytes, int bytesPerLine = 16,int startOffset = 0, int numBytes = -1)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length - startOffset;
            if (numBytes > 0)
                bytesLength = numBytes - startOffset;
            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = startOffset; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
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
    public delegate void EventHandler<TEventType, TEventArg1, TEventArg2>(TEventType sender, TEventArg1 arg1, TEventArg2 e);
    public delegate void EventHandler<TEventType, TEventArg1, TEventArg2, TEventArg3>(TEventType sender, TEventArg1 arg1, TEventArg2 arg2, TEventArg3 e);
    public static class SPADExtensions
    {
        #region Enums

        public static T SafeParse<T>(this Enum value, string valIn)
        {
            try
            {
                return (T)Enum.Parse(value.GetType(), valIn, true);
            }
            catch 
            {
                return default(T);
            }
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

            return (valueAsInt & flagAsInt) == valueAsInt;

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

    public static class TypeHelperExtensions
    {
        /// <summary>
        /// If the given <paramref name="type"/> is an array or some other collection
        /// comprised of 0 or more instances of a "subtype", get that type
        /// </summary>
        /// <param name="type">the source type</param>
        /// <returns></returns>
        public static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            var elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            var elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
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
        public static Double Rescale(this Double value, Double sourceMin, Double sourceMax, Double targetMin, Double targetMax)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rescale(this int value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            float val = Math.Max(sourceMin, Math.Min(value, sourceMax));
            
            return (int)( ((val - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin );
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
            foreach (var value in hashtable.Where(v=> p(v)).Select(v =>v.Key).ToList())
                hashtable.Remove(value);
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
            if ((key == null))
                return false;
            return dict.TryRemove(key, out var _);
        }

        public static bool Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (key == null)
                return false;
            if (dict.ContainsKey(key))
                dict.Remove(key);
            return dict.TryAdd(key, value);
        }
    }

    public interface INotifyPropertyChangedHandler : INotifyPropertyChanged
    {
        void OnNotifyPropertyChanged(string propertyName);
    }

    public struct BitArray64 : IEquatable<BitArray64>
    {

        /// <summary>
        /// Integer whose bits make up the array
        /// </summary>
        public ulong Bits;
        public ulong MaxValue;

        /// <summary>
        /// Create the array with the given number of bits
        /// </summary>
        /// 
        /// <param name="bits">
        /// Bits to make up the array
        /// </param>
        public BitArray64(int bits)
        {
            Bits = 0;
            Length = bits;
            if (bits == 64)
                MaxValue = ulong.MaxValue;
            else
                MaxValue = (1UL << bits) - 1;
        }

        public bool IsMax() => Bits == MaxValue;
        public void Clear() => Bits = 0;

        /// <summary>
        /// Get or set the bit at the given index. For faster getting of multiple
        /// bits, use <see cref="GetBits(ulong)"/>. For faster setting of single
        /// bits, use <see cref="SetBit(int)"/> or <see cref="UnsetBit(int)"/>. For
        /// faster setting of multiple bits, use <see cref="SetBits(ulong)"/> or
        /// <see cref="UnsetBits(ulong)"/>.
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to get or set
        /// </param>
        public bool this[int index]
        {
            get
            {
                RequireIndexInBounds(index);
                ulong mask = 1ul << index;
                return (Bits & mask) == mask;
            }
            set
            {
                RequireIndexInBounds(index);
                ulong mask = 1ul << index;
                if (value)
                {
                    Bits |= mask;
                }
                else
                {
                    Bits &= ~mask;
                }
            }
        }

        /// <summary>
        /// Get the length of the array
        /// </summary>
        /// 
        /// <value>
        /// The length of the array. Always 64.
        /// </value>
        public int Length;

        public override string ToString()
        {
            var sb = new StringBuilder(64);
            for (int i = 0; i < Length; i++)
            {
                sb.Append(( (Bits & ((ulong)(1ul << i))) != 0ul)? "1" : "0");
            }
            return sb.ToString();
        }
        /// <summary>
        /// Set a single bit to 1
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to set. Asserts if not on [0:31].
        /// </param>
        public void SetBit(int index)
        {
            RequireIndexInBounds(index);
            ulong mask = 1ul << index;
            Bits |= mask;
        }

        /// <summary>
        /// Set a single bit to 0
        /// </summary>
        /// 
        /// <param name="index">
        /// Index of the bit to unset. Asserts if not on [0:31].
        /// </param>
        public void UnsetBit(int index)
        {
            RequireIndexInBounds(index);
            ulong mask = 1ul << index;
            Bits &= ~mask;
        }

        /// <summary>
        /// Get all the bits that match a mask
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to get
        /// </param>
        /// 
        /// <returns>
        /// The bits that match the given mask
        /// </returns>
        public ulong GetBits(ulong mask)
        {
            return Bits & mask;
        }

        /// <summary>
        /// Set all the bits that match a mask to 1
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to set
        /// </param>
        public void SetBits(ulong mask)
        {
            Bits |= mask;
        }

        /// <summary>
        /// Set all the bits that match a mask to 0
        /// </summary>
        /// 
        /// <param name="mask">
        /// Mask of bits to unset
        /// </param>
        public void UnsetBits(ulong mask)
        {
            Bits &= ~mask;
        }

        /// <summary>
        /// Check if this array equals an object
        /// </summary>
        /// 
        /// <param name="obj">
        /// Object to check. May be null.
        /// </param>
        /// 
        /// <returns>
        /// If the given object is a BitArray64 and its bits are the same as this
        /// array's bits
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is BitArray64 && Bits == ((BitArray64)obj).Bits;
        }

        /// <summary>
        /// Check if this array equals another array
        /// </summary>
        /// 
        /// <param name="arr">
        /// Array to check
        /// </param>
        /// 
        /// <returns>
        /// If the given array's bits are the same as this array's bits
        /// </returns>
        public bool Equals(BitArray64 arr)
        {
            return Bits == arr.Bits;
        }

        /// <summary>
        /// Get the hash code of this array
        /// </summary>
        /// 
        /// <returns>
        /// The hash code of this array, which is the same as
        /// the hash code of <see cref="Bits"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Bits.GetHashCode();
        }

        

        /// <summary>
        /// Assert if the given index isn't in bounds
        /// </summary>
        /// 
        /// <param name="index">
        /// Index to check
        /// </param>
        public void RequireIndexInBounds(int index)
        {
            Debug.Assert(
                index >= 0 && index < Length ,
                "Index out of bounds: " + index);
        }
   }
}

namespace System.Threading.Tasks
{
    public static class TaskDelay
    {
        public static Task<bool> Wait(TimeSpan timeout, CancellationToken token) =>
            Task.Delay(timeout, token).ContinueWith(tsk => tsk.Exception == default);

        public static Task<bool> Wait(int timeoutMs, CancellationToken token) =>
            Task.Delay(timeoutMs, token).ContinueWith(tsk => tsk.Exception == default);
    }
}


