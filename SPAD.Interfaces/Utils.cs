﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public static class EnvironmentEx
    {
        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        public static TimeSpan UpTime
        {
            get { return TimeSpan.FromMilliseconds(GetTickCount64()); }
        }

        public static UInt64 TickCount64
        {
            get { return GetTickCount64(); }
        }

        public static UInt64 TickCount
        {
            get { return GetTickCount64(); }
        }

        public static long TickCountLong
        {
            get { return (long)GetTickCount64(); }
        }
    }

    public class IntegerValue
    {
        public int Value { get; set; }

        public static implicit operator IntegerValue(int value)
        {
            return new IntegerValue { Value = value };
        }
    }

    public class DoubleValue
    {
        public double Value { get; set; }

        public static implicit operator DoubleValue(double value)
        {
            return new DoubleValue { Value = value };
        }
    }

    
}