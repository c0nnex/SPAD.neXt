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
        string Key { get; }
        int Order { get; }
        bool Restart { get; }
        string ValueString { get; set; }
        string DefaultValueString { get; }
        bool Hidden { get; set; }
        string ConfigurationClass { get; set; }
        string DependsOn { get; set; }
        string OptionGroup { get; set; }
        IReadOnlyList<string> Choices { get; }

        void AddChoice(string choice);
        void SetDirty();
    }
}
