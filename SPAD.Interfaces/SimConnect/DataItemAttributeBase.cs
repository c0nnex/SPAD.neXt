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

    public enum SIMCONNECT_INPUT_EVENT_TYPE : uint
    {
        DOUBLE,
        STRING
    }

    public interface ISimConnectInputEvent
    {
        string Name { get;} // 64
        ulong Hash { get; }
        SIMCONNECT_INPUT_EVENT_TYPE eType { get;  }
    }

    public interface ISimConnectInputEventValue
    {
        ulong Hash { get; set; }

        SIMCONNECT_INPUT_EVENT_TYPE eType { get; set; }

        double ValueDouble { get; set; }
        string ValueString { get; set; }

        object Value { get; }
    }
}
