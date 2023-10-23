using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{
    public class AxisEventValue : IConvertible
    {
        public int RawValue;
        public float NormalizedRawValue;
        public int NormalizedValue;
        public int InvertedRawValue;
        public int InvertedNormalizedValue;
        public float AxisValue;
        public IInputAxis Axis { get; }
        public AxisEventValue()
        {

        }

        public AxisEventValue(AxisInputEventArgs e) : this(e.Input as IInputAxis)
        {
        }

        public AxisEventValue(IInputAxis axis)
        {
            Axis = axis;
            if (axis == null)
                return;
            RawValue = axis.RawValue;
            NormalizedValue = (int)axis.Rescale(axis.Value, 0, 1, 16383, -16383);
            InvertedRawValue = axis.MaximumValue - axis.RawValue + axis.MinimumValue;
            InvertedNormalizedValue = (int)axis.Rescale(1 - axis.Value, 0, 1, 16383, -16383);
            AxisValue = axis.Value;
            NormalizedRawValue = axis.Normalize(RawValue, axis.MinimumValue, axis.MaximumValue);
        }

        public AxisEventValue(IInputAxis axis, int rawValue, float value)
        {
            Axis = axis;
            RawValue = rawValue;
            NormalizedValue = (int)value.Rescale(0, 1, 16383, -16383);
            if (axis != null)
            {
                InvertedRawValue = axis.MaximumValue - rawValue + axis.MinimumValue;
            }
            InvertedNormalizedValue = (int)(1 - value).Rescale(0, 1, 16383, -16383);
            AxisValue = value;
        }

        public double GetEventValue(SPADValueOperation operation)
        {
            switch (operation)
            {
                case SPADValueOperation.Set:
                case SPADValueOperation.Increment:
                case SPADValueOperation.Decrement:
                case SPADValueOperation.SetBit:
                case SPADValueOperation.ClearBit:
                case SPADValueOperation.ToggleBit:
                case SPADValueOperation.AppendChars:
                case SPADValueOperation.DeleteChars:
                case SPADValueOperation.ClearChars:
                    return 0d;
                case SPADValueOperation.SetEventValue:
                    return RawValue;
                case SPADValueOperation.SetEventValue_Normalized:
                    return NormalizedValue;
                case SPADValueOperation.SetEventValue_Inverted:
                    return InvertedRawValue;
                case SPADValueOperation.SetEventValue_Normalized_Inverted:
                    return InvertedNormalizedValue;
                case SPADValueOperation.SetEventValue_AxisValue:
                    return AxisValue;
                case SPADValueOperation.SetEventValue_AxisPercent:
                    return Math.Max(Math.Min((int)(AxisValue * 100f), 100), 0);
                default:
                    break;
            }
            return 0d;
        }

        public int GetNormalizedValue(bool InvertAxis)
        {
            return InvertAxis ? InvertedNormalizedValue : NormalizedValue;
        }

        public int GetRawValue(bool InvertAxis)
        {
            return InvertAxis ? InvertedRawValue : RawValue;
        }

        public int GetValue(bool SendRaw, bool InvertAxis)
        {
            if (SendRaw)
                return InvertAxis ? InvertedRawValue : RawValue;
            else
                return InvertAxis ? InvertedNormalizedValue : NormalizedValue;
        }

       

        public double GetVariableValue()
        {
            int t = (int)(AxisValue * 100f);
            return t;
        }

        public override string ToString()
        {
            return $"AxisEventValue Value={RawValue} nValue={NormalizedValue} iValue={InvertedRawValue} inValue={InvertedNormalizedValue} aVal={AxisValue}";
        }

        public TypeCode GetTypeCode()
        {
            return RawValue.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return RawValue.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)RawValue).ToType(conversionType, provider);
        }
    }
}
