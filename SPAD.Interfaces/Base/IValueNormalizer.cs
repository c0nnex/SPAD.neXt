using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface IValueNormalizer
    {
        Guid ID { get; }
        bool HasAdditionalValue { get; }
        Double AdditionalValue { get; }

        bool HasMinimumChange { get; }
        Double MinimumChange { get; }

        TOut ConvertFrom<TOut>(object value);
        TOut ConvertFrom<TOut>(object value, object value1);
        TOut ConvertTo<TOut>(object value, bool secondary=false);
        Double ChangeValue(Double value, Double valChange);
        string FormatValue(object value);

        UInt16 SetBCDValue(object value, bool fractional);
        Double GetBCDValue(object value);

        void SetExpressionFrom(string expression);
        void SetExpressionTo(string expression);
    }

    public interface IDynamicNormalizer : IValueNormalizer
    {
        string Format { get; set; }
        string FromSim { get; set; }
        string Key { get; set; }
        string Name { get; set; }
        string ToSim { get; set; }
        bool Hidden { get; set; }

        void Construct();
    }
}
