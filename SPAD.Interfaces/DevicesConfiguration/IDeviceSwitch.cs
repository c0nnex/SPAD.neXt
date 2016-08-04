using SPAD.neXt.Interfaces.Base;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IGameDeviceInput
    {
        string InputName { get; }
        uint InputID { get; }
        JoystickInputTypes InputType { get; }
        
    }

    public interface IDeviceConfigValue : IGameDeviceInput
    {
        string Value { get; }
        int Order { get; }
        string Default { get; }
        string DefaultValue { get; }
        string DisableConfig { get; }
        string DisplayName { get; }       
        bool ReadOnly { get; }
        bool CanAdd { get; }
        bool CanEdit { get; }
        bool CanDelete { get; }
        bool CanBeBound { get; }
        bool Monitor { get; }
        string ActionTarget { get; }
        bool Selectable { get; }
        bool MustHaveCondition { get; }
        bool HasDisplayName { get; }
        bool CanChangeBehavior { get; }
        bool CanChangeIgnorePower { get;  }
        Guid SingletonID { get; }
        bool CanRename { get;  } 
        string Parameter { get; }
        SPADEventValueComparator ComparatorOverride { get; }
        IReadOnlyList<ISerializableOption> Options { get; }
        IReadOnlyList<string> DisabledConfigs { get; }
        IReadOnlyList<string> EnabledConfigs { get; }

        bool IsConfigAllowed(string id);
    }

    public interface IDeviceSwitch : IGameDeviceInput
    {
       
        string Inherit { get; }
        string Name { get; }

        bool ReadOnly { get; }
        string ConfigurationMode { get; }
        bool HasSettings { get; }
        bool CanRename { get; }

        int InputSubPanel { get; }
        int InputMode { get; }
        List<uint> InputVirtualHat { get; }

        IReadOnlyList<IDeviceConfigValue> ConfigValues { get; }
        IReadOnlyList<IDeviceSwitchConfiguration> SwitchConfigurations { get; }

        ISerializableOption GetOption(string optionName, string defaultValue = null);

        
    }

    public interface ISimpleSwitch : IDeviceSwitch
    {

    }

    public interface IRotarySwitch : IDeviceSwitch
    {

    }

}
