using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{

#if NET5_0_OR_GREATER
    [module: SkipLocalsInit]
#endif
    /// <summary>
    /// Abstraction of a stream of bytes.
    /// </summary>
    public interface IByteStream
    {
        /// <summary>
        /// Read a byte from the stream at a given index.
        /// </summary>
        /// <param name="index">Index of the byte to read.</param>
        /// <returns>The byte at the given index.</returns>
        byte Read(int index);

        /// <summary>
        /// Write a byte at a given index.
        /// </summary>
        /// <param name="value">Byte to write.</param>
        /// <param name="index">Index of the byte to write.</param>
        void Write(byte value, int index);
    }

    public class ByteArrayStream : IByteStream
    {
        byte[] data;

        public ByteArrayStream(byte[] data)
        {
            this.data = data;
        }

        public byte Read(int index)
        {
            if (index  < 0 || index >= data.Length)
                throw new ArgumentOutOfRangeException("index");
            return data[index];
        }

        public void Write(byte value, int index)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// High performance stack based reader and writer of a stream of bits which allows for 
    /// accessing the bits regardless of their size (up to 64 bits), even if the accesses cross
    /// one or more byte boundaries. This can be used to efficiently pack bits such that
    /// individual values can be stored in less than one byte.
    ///
    /// Inspired and improved upon blog article `Sub-Byte Size` by Jackson Dunstan
    /// <a href="https://www.jacksondunstan.com/articles/5426"/>
    /// Source: https://github.com/Sewer56/Sewer56.BitStream
    /// </summary>
    public class BitStream<TByteStream> where TByteStream : IByteStream
    {
        /*
            Notes:

            A.

            The type constraint above is very important.

            It saves us a virtual method call, as the read function on the Stream is used
            directly (and possibly even inlined) as opposed to using the interface.

            B.

            Comparisons of constants against constants e.g. Read<int>(4) 
            are optimized out by the JIT in Release mode. The checks/branches do not
            exist after JIT-ting if the parameter value is known at JIT-time.
        */

        internal const int ByteNumBits = sizeof(byte) * 8;
        internal const int ShortNumBits = sizeof(short) * 8;
        internal const int IntNumBits = sizeof(int) * 8;
        internal const int LongNumBits = sizeof(long) * 8;

        /// <summary>
        /// Absolute index of the next bit to access.
        /// </summary>
        public int BitIndex { get; set; }

        /// <summary>
        /// The current byte offset.
        /// </summary>
        public int ByteOffset => BitIndex / ByteNumBits;

        /// <summary>
        /// The current bit offset.
        /// </summary>
        public int BitOffset => BitIndex % ByteNumBits;

        /// <summary>
        /// Returns the index of the next byte to be read/written.
        /// If bit offset within the current byte is 0, this value is equal to <see cref="ByteOffset"/>.
        /// Else the value is <see cref="ByteOffset"/> + 1.
        /// </summary>
        public int NextByteIndex
        {
            get
            {
                bool extraByte = BitOffset != 0;
                return ByteOffset + (extraByte ? 1 : 0); // Branchless
            }
        }

        /// <summary>
        /// The stream to read from.
        /// </summary>
        public TByteStream Stream
        {
            get => _stream;
            private set => _stream = value;
        }

        private TByteStream _stream;

        /// <summary>
        /// Constructs a new instance of the BitStream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bitIndex">The bit index.</param>
        public BitStream(TByteStream stream, int bitIndex = default)
        {
            BitIndex = bitIndex;
            Stream = stream;
        }

        /// <summary>
        /// Reads a single bit from the stream.
        /// </summary>
        /// <returns>The read value, stored in the least-significant bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadBit()
        {
            const int bitCount = 1;
            const uint mask = 0b01;

            // Calculate where we are in the stream and advance.
            int bitIndex = BitIndex;
            int byteOffset = bitIndex / ByteNumBits;
            int bitOffset = bitIndex % ByteNumBits;
            BitIndex += bitCount;

            // Read the byte containing the bit we want and mask.
            byte highByte = Stream.Read(byteOffset);
            int highBitsAfterRead = ByteNumBits - bitOffset - bitCount;
            uint highMask = mask << highBitsAfterRead;
            return (byte)((highByte & highMask) >> highBitsAfterRead);
        }

        /// <summary>
        /// Read up to 8 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="numBits">Number of bits to read.</param>
        /// <returns>The read value, stored in the least-significant bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read8(int numBits)
        {
            // Calculate where we are in the stream and advance.
            int bitIndex = BitIndex;
            int byteOffset = bitIndex / ByteNumBits;
            int bitOffset = bitIndex % ByteNumBits;
            BitIndex = bitIndex + numBits;

            // Read the byte with the high bits (first byte) and decide if that's the only byte.
            byte highByte = Stream.Read(byteOffset);
            int highBitsAvailable = ByteNumBits - bitOffset;
            int highBitsAfterRead = highBitsAvailable - numBits;
            uint highMask;

            if (highBitsAfterRead >= 0)
            {
                highMask = GetMask(numBits) << highBitsAfterRead;
                return (byte)((highByte & highMask) >> highBitsAfterRead);
            }

            // Read the low byte and combine with the high byte.
            highMask = GetMask(highBitsAvailable);

            int numLowBits = numBits - highBitsAvailable;
            int lowShift = ByteNumBits - numLowBits;
            uint lowMask = GetMask(numLowBits) << lowShift;

            byte lowByte = Stream.Read(byteOffset + 1);
            uint highPart = (highByte & highMask) << numLowBits;
            uint lowPart = (lowByte & lowMask) >> lowShift;
            return (byte)(highPart | lowPart);
        }

        /// <summary>
        /// Read up to 16 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="numBits">Number of bits to read.</param>
        /// <returns>The read value, stored in the least-significant bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Read16(int numBits)
        {
            if (numBits <= ByteNumBits)
                return Read8(numBits);

            byte high = Read8(ByteNumBits);
            int numLowBits = numBits - ByteNumBits;
            byte low = Read8(numLowBits);
            return (ushort)((high << numLowBits) | low);
        }

        /// <summary>
        /// Read up to 32 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="numBits">Number of bits to read.</param>
        /// <returns>The read value, stored in the least-significant bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Read32(int numBits)
        {
            if (numBits <= ShortNumBits)
                return Read16(numBits);

            uint high = Read16(ShortNumBits);
            int numLowBits = numBits - ShortNumBits;
            uint low = Read16(numLowBits);
            return (high << numLowBits) | low;
        }

        /// <summary>
        /// Read up to 64 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="numBits">Number of bits to read.</param>
        /// <returns>The read value, stored in the least-significant bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Read64(int numBits)
        {
            if (numBits <= IntNumBits)
                return Read32(numBits);

            ulong high = Read32(IntNumBits);
            int numLowBits = numBits - IntNumBits;
            ulong low = Read32(numLowBits);
            return (high << numLowBits) | low;
        }
        /*
        /// <summary>
        /// Reads a specified type (up to 64 bits) from the stream.
        /// </summary>
        /// <typeparam name="T">Any of the following: byte, sbyte, short, ushort, int, uint, long, ulong.</typeparam>
        /// <param name="numBits">Number of bits to read. Max: 64.</param>
        /// <remarks>Using this method has no additional overhead compared to the other methods in Release mode.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ExcludeFromCodeCoverage]
        public T Read<T>(int numBits)
        {
            if (typeof(T) == typeof(byte)) return (T)Read8(numBits);
            else if (typeof(T) == typeof(sbyte)) return Number.Cast<T>(Read8(numBits));

            else if (typeof(T) == typeof(short)) return Number.Cast<T>(Read16(numBits));
            else if (typeof(T) == typeof(ushort)) return Number.Cast<T>(Read16(numBits));

            else if (typeof(T) == typeof(int)) return Number.Cast<T>(Read32(numBits));
            else if (typeof(T) == typeof(uint)) return Number.Cast<T>(Read32(numBits));

            else if (typeof(T) == typeof(long)) return Number.Cast<T>(Read64(numBits));
            else if (typeof(T) == typeof(ulong)) return Number.Cast<T>(Read64(numBits));

#if DEBUG
            // Debug-only because exceptions prevent inlining.
            else throw new InvalidCastException();
#else
        else return default;
#endif
        }
        */
        /// <summary>
        /// [ASSUMES ALIGNMENT. INCORRECT USE IF <see cref="BitIndex"/> IS NOT MULTIPLE OF 8]
        /// Reads a byte from the stream.
        /// </summary>
        /// <returns>The read value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read8Aligned()
        {
            // Calculate where we are in the stream and advance.
            int byteOffset = BitIndex / ByteNumBits;
            var result = Stream.Read(byteOffset);
            BitIndex += ByteNumBits;
            return result;
        }

        /// <summary>
        /// [ASSUMES ALIGNMENT. INCORRECT USE IF <see cref="BitIndex"/> IS NOT MULTIPLE OF 8]
        /// Reads a short from the stream.
        /// </summary>
        /// <returns>The read value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort Read16Aligned()
        {
            // Calculate where we are in the stream and advance.
            int byteOffset = BitIndex / ByteNumBits;
            var high = Stream.Read(byteOffset) << 8;
            var low = Stream.Read(byteOffset + 1);
            BitIndex += ShortNumBits;
            return (ushort)(high | low);
        }

        /// <summary>
        /// [ASSUMES ALIGNMENT. INCORRECT USE IF <see cref="BitIndex"/> IS NOT MULTIPLE OF 8]
        /// Reads a int from the stream.
        /// </summary>
        /// <returns>The read value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Read32Aligned()
        {
            // Calculate where we are in the stream and advance.
            int byteOffset = BitIndex / ByteNumBits;
            var b0 = Stream.Read(byteOffset) << 24;
            var b1 = Stream.Read(byteOffset + 1) << 16;
            var b2 = Stream.Read(byteOffset + 2) << 8;
            var b3 = Stream.Read(byteOffset + 3);
            BitIndex += IntNumBits;
            return (uint)(b0 | b1 | b2 | b3);
        }

        /// <summary>
        /// [ASSUMES ALIGNMENT. INCORRECT USE IF <see cref="BitIndex"/> IS NOT MULTIPLE OF 8]
        /// Reads a long from the stream.
        /// </summary>
        /// <returns>The read value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Read64Aligned()
        {
            // Note: This could be better optimised on x86 but for now it's ok; this is a cold path.
            // Calculate where we are in the stream and advance.
            int byteOffset = BitIndex / ByteNumBits;
            var b0 = (ulong)Stream.Read(byteOffset) << 56;
            var b1 = (ulong)Stream.Read(byteOffset + 1) << 48;
            var b2 = (ulong)Stream.Read(byteOffset + 2) << 40;
            var b3 = (ulong)Stream.Read(byteOffset + 3) << 32;

            b0 = b0 | b1 | b2 | b3;
            var b4 = (ulong)Stream.Read(byteOffset + 4) << 24;
            var b5 = (ulong)Stream.Read(byteOffset + 5) << 16;
            var b6 = (ulong)Stream.Read(byteOffset + 6) << 8;
            var b7 = (ulong)Stream.Read(byteOffset + 7);

            b4 = b4 | b5 | b6 | b7;
            BitIndex += LongNumBits;
            return b0 | b4;
        }

        

        /// <summary>
        /// Writes a single bit starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBit(byte value)
        {
            const int numBits = 1;
            const int singleBitMask = 0b01;

            // Calculate where we are in the stream and advance.
            int bitIndex = BitIndex;
            int localByteIndex = bitIndex / ByteNumBits;
            int localBitIndex = bitIndex % ByteNumBits;
            BitIndex = bitIndex + numBits;

            // Read the first byte, mix with our value and write back.
            byte firstByte = Stream.Read(localByteIndex);
            int firstBitsAvailable = ByteNumBits - localBitIndex;
            int firstBitsAfterWrite = firstBitsAvailable - numBits;
            var firstMask = (GetMask(localBitIndex) << firstBitsAvailable) | GetMask(firstBitsAfterWrite); // Masks out bits except those to write.
            uint valueMask = singleBitMask;
            Stream.Write((byte)((firstByte & firstMask) | ((value & valueMask) << firstBitsAfterWrite)), localByteIndex);
        }

        /// <summary>
        /// Write up to 8 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="numBits">Number of bits to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write8(byte value, int numBits)
        {
            // Calculate where we are in the stream and advance.
            int bitIndex = BitIndex;
            int localByteIndex = bitIndex / ByteNumBits;
            int localBitIndex = bitIndex % ByteNumBits;
            BitIndex = bitIndex + numBits;

            // When overwriting a whole byte, there's no need to read existing
            // bytes and mix in the written bits.
            if (localBitIndex == 0 && numBits == ByteNumBits)
            {
                Stream.Write(value, localByteIndex);
                return;
            }

            // Read the first byte and decide if that's the only byte.
            byte firstByte = Stream.Read(localByteIndex);
            int firstBitsAvailable = ByteNumBits - localBitIndex;
            int firstBitsAfterWrite = firstBitsAvailable - numBits;
            uint valueMask;
            uint firstMask; // Masks out bits except those to write.
            if (firstBitsAfterWrite >= 0)
            {
                firstMask = (GetMask(localBitIndex) << firstBitsAvailable) | GetMask(firstBitsAfterWrite);
                valueMask = GetMask(numBits);
                Stream.Write((byte)((firstByte & firstMask) | ((value & valueMask) << firstBitsAfterWrite)), localByteIndex);
                return;
            }

            // Combine the high bits of the value to write with the high bits of the first byte and write.
            int numLowBits = numBits - firstBitsAvailable;
            valueMask = GetMask(firstBitsAvailable);
            firstMask = ~valueMask;
            Stream.Write((byte)((firstByte & firstMask) | (value >> numLowBits) & valueMask), localByteIndex);

            // Read the second byte and combine its low bits with the low bits of the value to write.
            byte secondByte = Stream.Read(localByteIndex + 1);

            valueMask = GetMask(numLowBits);
            int numSecond = ByteNumBits - numLowBits;
            uint secondMask = GetMask(numSecond);
            Stream.Write((byte)(((value & valueMask) << numSecond) | (secondByte & secondMask)), localByteIndex + 1);
        }

        /// <summary>
        /// Write up to 16 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="numBits">Number of bits to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write16(ushort value, int numBits)
        {
            if (numBits <= ByteNumBits)
            {
                Write8((byte)value, numBits);
                return;
            }

            int numLow = numBits - ByteNumBits;
            Write8((byte)((value & (byte.MaxValue << numLow)) >> numLow), ByteNumBits);
            Write8((byte)(value & GetMask(numLow)), numLow);
        }

        /// <summary>
        /// Write up to 32 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="numBits">Number of bits to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write32(uint value, int numBits)
        {
            if (numBits <= ShortNumBits)
            {
                Write16((ushort)value, numBits);
                return;
            }

            int numLow = numBits - ShortNumBits;
            Write16((ushort)((value & (ushort.MaxValue << numLow)) >> numLow), ShortNumBits);
            Write16((ushort)(value & GetMask(numLow)), numLow);
        }

        /// <summary>
        /// Write up to 64 bits starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="numBits">Number of bits to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write64(ulong value, int numBits)
        {
            if (numBits <= IntNumBits)
            {
                Write32((uint)value, numBits);
                return;
            }

            int numLow = numBits - IntNumBits;
            Write32((uint)((value & (uint.MaxValue << numLow)) >> numLow), IntNumBits);
            Write32((uint)(value & GetMaskLong(numLow)), numLow);
        }

        /// <summary>
        /// [ASSUMES ALIGNMENT. INCORRECT USE IF <see cref="BitIndex"/> IS NOT MULTIPLE OF 8]
        /// Writes a byte to the position starting at <see cref="BitIndex"/>.
        /// </summary>
        /// <param name="value">Value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write8Aligned(byte value)
        {
            // Calculate where we are in the stream and advance.
            int localByteIndex = BitIndex / ByteNumBits;
            Stream.Write(value, localByteIndex);
            BitIndex += ByteNumBits;
        }

       
       
        #region Extensions

        /// <summary>
        /// Seeks the stream to a specified location.
        /// </summary>
        /// <param name="offset">The byte offset.</param>
        /// <param name="bit">The bit value to add to the byte offset.</param>
        public void Seek(int offset, byte bit = 0) => BitIndex = (offset * ByteNumBits) + bit;

        /// <summary>
        /// Seeks the stream to a specified location.
        /// </summary>
        /// <param name="offset">The byte offset.</param>
        /// <param name="bit">The bit value to add to the byte offset.</param>
        public void SeekRelative(int offset = 0, byte bit = 0) => BitIndex += (offset * ByteNumBits) + bit;

        
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the mask necessary to mask out a given number of bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetMask(int numBits) => ((uint)1 << numBits) - 1;

        /// <summary>
        /// Gets the mask necessary to mask out a given number of bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong GetMaskLong(int numBits) => ((ulong)1 << numBits) - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SignShrink(int value) => SignExtend(value, IntNumBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long SignShrink(long value) => SignExtend(value, LongNumBits);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SignExtend(int value, int numBits)
        {
            var mask = 1 << (numBits - 1);
            return (value ^ mask) - mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long SignExtend(long value, int numBits)
        {
            long mask = 1L << (numBits - 1);
            return (value ^ mask) - mask;
        }
        #endregion
    }
}
