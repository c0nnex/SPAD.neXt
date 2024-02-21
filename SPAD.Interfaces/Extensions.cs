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
using static System.Windows.Forms.AxHost;
using System.Globalization;
using SPAD.neXt.Interfaces;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Controls;

namespace System
{
    public static class SPADSystemExtensions
    {
        public static string GetMD5(this string s)
        {
            var data = Encoding.UTF8.GetBytes(s);
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLower();
            }
        }

        public static string GetMD5(this byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLower();
            }
        }

        public static string GetFileMD5(this string filename)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        string s = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToUpper();                        
                        return s;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public static bool IsEmpty(this Guid g)
        {
            return ((g == null) || g == Guid.Empty);
        }
        public static string AsciiBytesToString(this byte[] buffer, int offset, int maxLength)
        {
            int maxIndex = offset + maxLength;

            for (int i = offset; i < maxIndex; i++)
            {
                /// Skip non-nulls.
                if (buffer[i] != 0) continue;
                /// First null we find, return the string.
                return Encoding.ASCII.GetString(buffer, offset, i - offset);
            }
            /// Terminating null not found. Convert the entire section from offset to maxLength.
            return Encoding.ASCII.GetString(buffer, offset, maxLength);
        }

        public static string Right(this string inStr, int numChars)
        {
            if (String.IsNullOrEmpty(inStr) || (numChars <= 0))
                return String.Empty;
            return inStr.Substring(inStr.Length - Math.Min(numChars, inStr.Length), Math.Min(numChars, inStr.Length));
        }

        public static string Left(this string inStr, int numChars, int startIndex = 0)
        {
            if (String.IsNullOrEmpty(inStr) || (numChars <= 0))
                return String.Empty;
            int ct = Math.Min(numChars, inStr.Length - startIndex);
            if (ct <= 0)
                return String.Empty;
            return inStr.Substring(startIndex, ct);
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
                    res = !(String.IsNullOrEmpty(parts[part]) || parts[part] == "0" || String.Compare(parts[part], "false", true) == 0);
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
        public static string ToUTF8String(this byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
                return string.Empty;
            var value = Encoding.UTF8.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }

        public static string ToUTF16String(this byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
                return string.Empty;
            var value = Encoding.Unicode.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }
        public static String ToHumanReadableSize(this object value)
        {
            if (value == null)
                return String.Empty;
            string[] suf = { "b", "kb", "mb", "gb", "tb", "pb", "eb" }; //Longs run out around EB
            long byteCount = Convert.ToInt64(value);
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static string NomalizeHexNumber(this string input)
        {
            if (input == null)
                return "FFFF";
            if (input.StartsWith("0x"))
            {
                try
                {
                    return Int32.Parse(input.Substring(2), System.Globalization.NumberStyles.HexNumber).ToString("X4");
                }
                catch
                {
                    return input.Substring(2);
                }
            }
            return input.ToUpperInvariant();
        }

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ReplaceCaseInsensitive(this string str, string oldValue, string newValue)
        {
            int prevPos = 0;
            string retval = str;
            // find the first occurence of oldValue
            int pos = retval.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase);

            while (pos > -1)
            {
                // remove oldValue from the string
                retval = retval.Remove(pos, oldValue.Length);

                // insert newValue in it's place
                retval = retval.Insert(pos, newValue);

                // check if oldValue is found further down
                prevPos = pos + newValue.Length;
                pos = retval.IndexOf(oldValue, prevPos, StringComparison.InvariantCultureIgnoreCase);
            }

            return retval;
        }

        public static bool CompareNoCase(this string s, string other)
        {
            return !string.IsNullOrEmpty(s) && (string.Compare(s, other, true) == 0);
        }

        public static bool CaseInsensitiveCompare(this string s, string other)
        {
            return !string.IsNullOrEmpty(s) && (string.Compare(s, other, true) == 0);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNull(this object o)
        {
            return (o == null);
        }

        public static bool ContainsNoCase(this IEnumerable<string> list, string value)
        {
            return list.FirstOrDefault(t => string.Compare(t, value, true) == 0) != null;
        }

       
        public static string ToShortDateTimeString(this DateTime dt)
        {
            return $"{dt.ToShortDateString()} {dt.ToShortTimeString()}";
        }

        public static string ToTimeSpanString(this DateTime dt)
        {
            return new TimeSpan(0, dt.Hour, dt.Minute, dt.Second, dt.Millisecond).ToString();
        }
        public static String[] Split(this string inStr, String separator)
        {
            return inStr.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Guid ToGuid(this object obj)
        {
            if (obj == null) return Guid.Empty;
            if (Guid.TryParse(obj.ToString(), out Guid guid)) return guid;
            return Guid.Empty;
        }
        public static string HexDump(this byte[] bytes, int bytesPerLine = 16, int startOffset = 0, int numBytes = -1, bool noAscii = false)
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
                        if (!noAscii)
                            line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        if (!noAscii)
                            line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }
        public static T GetValueAs<T>(this object obj,T defValue = default) 
        {
            try
            {
                if (obj == null)
                    return defValue;
                object tObj = obj;
                if (obj is IGetValueOverride tOverride)
                {
                    tObj = tOverride.GetValueTarget();
                }
                var Value = Convert.ToString(tObj, CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(Value))
                    return defValue;
                object res;
                if (typeof(T) == typeof(Guid))
                {
                    res = Guid.Parse(Value);
                    return (T)res;
                }
                if (typeof(T) == typeof(bool))
                {
                    res = !(Value == "0" || String.IsNullOrEmpty(Value) || String.Compare(Value, "false", true) == 0);
                    return (T)res;
                }
                if (typeof(T) == typeof(char))
                {
                    res = Value.FirstOrDefault();
                    return (T)res;
                }
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), Value, true);

                res = Convert.ChangeType(Value, typeof(T), CultureInfo.InvariantCulture);
                return (T)res;
            }
            catch
            {
                return defValue;
            }
        }
    }

    public static class StringExtensions
    {

        public static bool IsValidNumeric(this string str, bool allowWhitespace = false)
        {
            if (String.IsNullOrEmpty(str))
                return false;
            if (allowWhitespace)
                str = str.Trim(); // trims the white spaces.

            if (str.Length == 0)
                return false;

            // if string is of length 1 and the only
            // character is not a digit
            if (str.Length == 1 && !char.IsDigit(str[0]))
                return false;

            // If the 1st char is not '+', '-', '.' or digit
            if (str[0] != '+' && str[0] != '-'
                && !char.IsDigit(str[0]) && str[0] != '.')
                return false;

            // To check if a '.' or 'e' is found in given
            // string. We use this flag to make sure that
            // either of them appear only once.
            Boolean flagDotOrE = false;

            for (int i = 1; i < str.Length; i++)
            {
                // If any of the char does not belong to
                // {digit, +, -, ., e}
                if (!char.IsDigit(str[i]) && str[i] != 'e'
                    && str[i] != '.' && str[i] != '+'
                    && str[i] != '-')
                    return false;

                if (str[i] == '.')
                {

                    // checks if the char 'e' has already
                    // occurred before '.' If yes, return 0.
                    if (flagDotOrE == true)
                        return false;

                    // If '.' is the last character.
                    if (i + 1 >= str.Length)
                        return false;

                    // if '.' is not followed by a digit.
                    if (!char.IsDigit(str[i + 1]))
                        return false;
                }

                else if (str[i] == 'e')
                {
                    // set flagDotOrE = 1 when e is encountered.
                    flagDotOrE = true;

                    // if there is no digit before 'e'.
                    if (!char.IsDigit(str[i - 1]))
                        return false;

                    // If 'e' is the last Character
                    if (i + 1 >= str.Length)
                        return false;

                    // if e is not followed either by
                    // '+', '-' or a digit
                    if (!char.IsDigit(str[i + 1]) && str[i + 1] != '+'
                        && str[i + 1] != '-')
                        return false;
                }
            }

            /* If the string skips all above cases, 
            then it is numeric*/
            return true;
        }

        public static bool ContainsNoCase(this string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        /// <summary>
        /// Remove all but Digits and Letters from string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CleanupString(this string s, bool allowWhiteSpace = true)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;
            var arr = s.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                if (!Char.IsLetterOrDigit(c) || (Char.IsWhiteSpace(c) && !allowWhiteSpace))
                    arr[i] = '_';
            }
            return new string(arr);
        }

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        public static string RemoveXmlNamespaces(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            return s.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
        }
        public static String ToHexString(this ulong num)
        {
            if (num <= uint.MaxValue)
                return String.Format("{0}:{1}", ((num & 0xFFFF0000) >> 16).ToString("X4"), (num & 0x0000ffff).ToString("X4"));
            return num.ToString("X");
        }

        public static String ToHexString(this UInt32 num)
        {
            return String.Format("{0}:{1}", ((num & 0xFFFF0000) >> 16).ToString("X4"), (num & 0x0000ffff).ToString("X4"));
        }
        public static String ToHexString(this Int32 num)
        {
            return String.Format("{0}:{1}", ((num & 0xFFFF0000) >> 16).ToString("X4"), (num & 0x0000ffff).ToString("X4"));
        }
        public static String ToHexString(this ushort num)
        {
            return String.Format("{0}", (num & 0x0000ffff).ToString("X4"));
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}

namespace System.Collections.Generic
{
    public class ClonableList<T> : List<T>, ICloneable<ClonableList<T>> where T : class, ICloneable<T>
    {
        public ClonableList()
        {
        }

        public ClonableList(IEnumerable<T> collection) : base(collection)
        {
        }

        public ClonableList<T> Clone()
        {
            var outVal = new ClonableList<T>();
            this.ForEach(item => outVal.Add(item.Clone()));
            return outVal;
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
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e);
    public delegate Task AsyncEventHandler<TEventType, TEventArgs>(TEventType sender, TEventArgs e);
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

        public static void FireAndForget<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs args)
        {
            _ = Task.Factory.StartNew(async state =>
            {
                try
                {
                    await handler(sender, (TEventArgs)(state ?? default));
                }
                catch (Exception ex)
                {
                    // handle the exception, notify the app
                }
            }, args);
        }
        public static void FireAndForget<TEventType, TEventArgs>(this AsyncEventHandler<TEventType, TEventArgs> handler, TEventType sender, TEventArgs args)
        {
            _ = Task.Factory.StartNew(async state =>
            {
                try
                {
                    await handler(sender, (TEventArgs)(state ?? default));
                }
                catch (Exception ex)
                {
                    // handle the exception, notify the app
                }
            }, args);
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
            if (sourceMin > sourceMax)
            {
                var tmp = sourceMin;
                sourceMin = sourceMax;
                sourceMax = tmp;
                tmp = targetMin;
                targetMin = targetMax;
                targetMax = tmp;
            }
            value = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return ((value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rescale(this int value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            float val = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return (int)(((val - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin);
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
            foreach (var value in hashtable.Where(v => p(v)).Select(v => v.Key).ToList())
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

        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, bool disposeVal = false)
        {
            if ((key == null))
                return false;

            if (dict.TryRemove(key, out var val))
            {
                if (disposeVal)
                {
                    if (val is IDisposable disposable)
                        disposable.Dispose();
                }
                return true;
            }
            return false;
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
                sb.Append(((Bits & ((ulong)(1ul << i))) != 0ul) ? "1" : "0");
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
                index >= 0 && index < Length,
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


