using SPAD.neXt.Interfaces.Base;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IGameDeviceInput
    {
        string InputName { get;set; }
        uint InputID { get;set; }
        JoystickInputTypes InputType { get;set; }
        bool NoCalibration { get;set; }
        
    }

    public interface IDeviceConfigValue : IGameDeviceInput
    {
        string Value { get;set; }
        int Order { get;set; }
        string Default { get;set; }
        string DefaultValue { get;set; }
        string DisableConfig { get;set; }
        string DisplayName { get;set; }       
        bool ReadOnly { get;set; }
        bool CanAdd { get;set; }
        bool CanEdit { get;set; }
        bool CanDelete { get;set; }
        bool CanBeBound { get;set; }
        bool Monitor { get;set; }
        string ActionTarget { get;set; }
        bool Selectable { get;set; }
        bool MustHaveCondition { get;set; }
        SPADConditionType ConditionType { get;set; }
        bool HasDisplayName { get; }
        bool CanChangeBehavior { get;set; }
        bool HasModes { get;set; }
        bool CanChangeTunerAcceleration { get;set; }
        bool CanChangeIgnorePower { get;  }
        Guid SingletonID { get;set; }
        bool IsSingleton { get; }
        bool CanRename { get;  } 
        string Parameter { get;set; }
        string EnableConfig { get; set; }
        SPADEventValueComparator ComparatorOverride { get; }
        IReadOnlyList<ISerializableOption> Options { get; }
        IReadOnlyList<string> DisabledConfigs { get; }
        IReadOnlyList<string> EnabledConfigs { get; }

        bool IsConfigAllowed(string id);
    }

    public interface IDeviceSwitch : IGameDeviceInput
    {
       
        string Inherit { get;set; }
        string Name { get;set; }
        SPADSwitchTypes SwitchType { get;}
        bool ReadOnly { get;set; }
        string ConfigurationMode { get;set; }
        bool HasSettings { get;set; }
        bool CanRename { get;set; }
        bool Selectable { get;set; }
        bool IsEnabled { get; set; }
        bool IsCustomized { get; set; }
        int InputSubPanel { get;set; }
        int InputMode { get;set; }
        bool NoCustomize { get; set; }
        bool IsAxis { get; }
        bool IsLever { get; }

        List<uint> InputVirtualHat { get; }

        IReadOnlyList<IDeviceConfigValue> ConfigValues { get; }
        IReadOnlyList<IDeviceSwitchConfiguration> SwitchConfigurations { get; }

        ISerializableOption GetPrivateOption(string optionName, string defaultValue = null);
        IDeviceConfigValue FindConfigValue(string trigger);
        void AddConfigValue(IDeviceConfigValue cfgValue);
        IDeviceConfigValue CreateConfigValue();
        ISerializableOption GetTriggerOption(string trigger, string optionName, string defaultValue = null);

        bool DoesInherit(string baseValue);
    }

    public interface ISimpleSwitch : IDeviceSwitch
    {

    }

    public interface IRotarySwitch : IDeviceSwitch
    {

    }

}
