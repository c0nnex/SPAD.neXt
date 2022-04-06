
using System;
using System.ComponentModel;

namespace SPAD.neXt.Interfaces.Profile
{
    public interface IProfileManager
    {
        SPAD.neXt.Interfaces.Profile.IProfile ActiveProfile { get; }
        event PropertyChangedEventHandler ActiveProfileChanged;
        event PropertyChangedEventHandler ActiveProfileChanging;
        event PropertyChangedEventHandler ActiveProfilePropertyChanged;
        event PropertyChangedEventHandler ActiveProfileStatusChanged;
        IExtensionProfileOption ProfileAddOption(string key, ProfileOptionTypes type, string defaultValue, bool changeNeedsRestart, int order);
        IExtensionProfileOption ProfileAddHiddenOption(string key, ProfileOptionTypes type, string defaultValue);
        IExtensionProfileOption DeviceAddOption(string deviceKey,string key, ProfileOptionTypes type, string defaultValue, bool changeNeedsRestart, int order);
        IExtensionProfileOption DeviceAddHiddenOption(string deviceKey, string key, ProfileOptionTypes type, string defaultValue);
        string ProfilePath { get; }
    }

   

    public enum ProfileOptionTypes
    {
        Boolean,
        String,
        Integer,
        Choice        
    }
}
