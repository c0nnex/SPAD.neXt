using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Base;

namespace SPAD.neXt.Interfaces.Configuration
{
    public interface IDataProvider
    {
        string Name { get; }
        DataProviderMapType MapType { get; set; }
        string MappedTo { get; }
        string WriteMode { get; }
        string Unit { get; }
        bool HasRange { get; }
        IValueNormalizer ValueNormalizer { get; }
        decimal CheckRange(decimal inVal);
    }
}