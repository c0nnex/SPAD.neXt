﻿using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public delegate void SPADEventHandler<TSender,TArgument>(TSender sender, TArgument e);
    public delegate void SPADEventHandler(object sender, ISPADEventArgs e);

    public sealed class BooleanEventArgs : EventArgs
    {
        public static readonly BooleanEventArgs True = new BooleanEventArgs(true);
        public static readonly BooleanEventArgs False = new BooleanEventArgs(false);
        public bool Value { get; } = false;
        public bool Value1 { get; } = false;
        public bool Value2 { get; } = false;
        public BooleanEventArgs()
        {

        }
        public BooleanEventArgs(bool value)
        {
            Value = value;
        }
        public BooleanEventArgs(bool value, bool value1)
        {
            Value = value;
            Value1 = value1;
        }
        public BooleanEventArgs(bool value, bool value1, bool value2)
        {
            Value = value;
            Value1 = value1;
            Value2 = value2;
        }
    }
    public class LedStatusEventArgs : EventArgs
    {
        public static readonly LedStatusEventArgs OFF = new LedStatusEventArgs(false);
        public bool IsOn { get; set; } = false;
        public FLASHMODE FlashMode { get; set; }
        public object AdditionalData { get; set; }

        public LedStatusEventArgs(bool isOn, FLASHMODE flashMode = FLASHMODE.FLASHMODE_STATIC, object additionalData = null)
        {
            IsOn = isOn;
            FlashMode = flashMode;
            AdditionalData = additionalData;
        }
    }
    public sealed class ValidationEventArgs : EventArgs
    {
        public bool IsValid { get; set; }
        public object Value { get; }
        public string Error { get; set; }

        public ValidationEventArgs()
        {

        }

        public ValidationEventArgs(object value) : this()
        {
            Value = value;
        }
    }
}
