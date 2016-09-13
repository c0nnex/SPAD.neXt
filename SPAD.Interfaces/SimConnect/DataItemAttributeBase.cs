using System;
namespace SPAD.neXt.Interfaces.SimConnect
{

    public interface IDataItemAttributeBase
    {
        int ItemIndex
        {
            get;
            set;
        }
        int ItemSize
        {
            get;
            set;
        }
        float ItemEpsilon
        {
            get;
            set;
        }
        int ItemOffset { get; set; }
        bool IsFixedSize { get; set; }
    }
    public interface IDataItemBoundBase
    {
         string DatumName { get; }
         string UnitsName { get; }
         string ID { get; }
    }
}
