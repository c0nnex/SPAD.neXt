using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{
    public class AxisEventValue
    {
        public int RawValue;
        public int NormalizedValue;
        public int InvertedRawValue;
        public int InvertedNormalizedValue;
        public float AxisValue;
        public IInputAxis Axis { get; }
        public AxisEventValue()
        {

        }

        public AxisEventValue(InputEventArgs e) : this( e.Input as IInputAxis )
        {
        }

        public AxisEventValue(IInputAxis axis)
        {
            Axis = axis;
            RawValue = axis.RawValue;
            NormalizedValue = (int)axis.Rescale(axis.Value, 0, 1, 16383, -16383);
            InvertedRawValue = axis.MaximumValue - axis.RawValue + axis.MinimumValue;
            InvertedNormalizedValue = (int)axis.Rescale(1 - axis.Value, 0, 1, 16383, -16383);
            AxisValue = axis.Value;
        }

        public AxisEventValue(IInputAxis axis, int rawValue, float value)
        {
            Axis = axis;
            RawValue = rawValue;
            NormalizedValue = (int)axis.Rescale(value, 0, 1, 16383, -16383);
            InvertedRawValue = axis.MaximumValue - rawValue + axis.MinimumValue;
            InvertedNormalizedValue = (int)axis.Rescale(1 - value, 0, 1, 16383, -16383);
            AxisValue = value;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Rescale(float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            value = Math.Max(sourceMin, Math.Min(value, sourceMax));

            return ((value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
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
    }
}
