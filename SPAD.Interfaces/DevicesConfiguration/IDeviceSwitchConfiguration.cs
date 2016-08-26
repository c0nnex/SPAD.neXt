using System;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IDeviceSwitchConfiguration
    {
        string ConfigControl { get; }
        string ConfigParam { get; }
        string Description { get; }
        string EventType { get; }
        string ID { get; }
        string Name { get; }
        string Connection { get; }
        string MasterKey { get; }
        bool MultiAction { get; }
        bool HasMinMax { get; }
        SPADEventActions ActionID { get; }
       
        T GetOption<T>(string optionName, T defaultValue = default(T));
        bool HasOption(string optionName);
    }
}
