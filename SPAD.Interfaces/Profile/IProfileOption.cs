using SPAD.neXt.Interfaces.Base;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.Profile
{
    public interface IProfileOption : ISerializableOption, IExtensionProfileOption
    {
    }

    public interface IExtensionProfileOption
    {
        //        string Key { get; }
        int Order { get; }
        bool Restart { get; }
        string ValueString { get; set; }
        string DefaultValueString { get; }
        bool Hidden { get; set; }
        string ConfigurationClass { get; set; }
        string ConfigurationName { get; set; }

        List<ExtensionConfigurationEvent> ConfigurationEvents { get; set; }

        string DependsOn { get; set; }
        string OptionGroup { get; set; }
        IReadOnlyList<string> Choices { get; }

        IExtensionProfileOption AddChoice(string choice);
        void SetDirty();
    }

    public class ExtensionConfigurationEvent
    {
        public string EventType { get; set; } = "Button";
        public string EventName { get; set; }
        public string EventTrigger { get; set; }
        public object EventValue { get; set; }
        public string EventDisplayName { get; set; }
        public Func<bool> IsEnabled { get; set; } = () => true;
        public Func<bool> IsVisible { get; set; } = () => true;
    }

}
