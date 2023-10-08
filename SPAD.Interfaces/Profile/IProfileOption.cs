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
        int Order { get; set; }
        bool Restart { get; set; }
        string ValueString { get; set; }
        string DefaultValueString { get; }
        bool Hidden { get; set; }
        string ConfigurationClass { get; set; }
        string ConfigurationName { get; set; }
        bool Editable { get; set; }
        string ValueType { get; set; }

        List<ExtensionConfigurationEvent> ConfigurationEvents { get; set; }

        string DependsOn { get; set; }
        string OptionGroup { get; set; }
        IReadOnlyList<string> Choices { get; }

        IExtensionProfileOption AddChoice(string choice);
        IExtensionProfileOption WithDependsOn(string dependsOn);
        IExtensionProfileOption WithOptionGroup(string group);

        IExtensionProfileOption WithOptionScope(string scope);

        IExtensionProfileOption AsHidden();
        void SetDirty();

        T GetValue<T>();
    }

    public class ExtensionConfigurationEvent
    {
        public Action<bool> Callback = (b) => { };
        public string EventType { get; set; } = "Button";
        public string EventName { get; set; }
        public string EventTrigger { get; set; }
        public object EventValue { get; set; }
        public string EventDisplayName { get; set; }
        public Func<string,bool> IsEnabled { get; set; } = (s) => true;
        public Func<bool> IsVisible { get; set; } = () => true;

        public void UpdateStatus()
        {
            Callback(IsEnabled(EventTrigger));
        }
    }

}
