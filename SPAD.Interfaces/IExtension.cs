
using SPAD.Extensions.Generic;
using SPAD.neXt.Interfaces.HID;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IExtension
    {        
        void StartupExtension(IApplication app);
        void InitializeExtension();
        void DeinitializeExtension();
        void ShutdownExtenion();
        bool IsExtensionEnabled(IApplication app);
        IExtensionInfo GetExtensionInformation();
        IPanelControl CreateControl(IExtensionPanel ctrl);
        void DisposeControl(IPanelControl ctrl);
        IApplicationConfiguration CreateConfiguration(string ctrl);
        Guid ID { get; }
    }

    public interface IExtensionDynamicDevice
    {
        GenericSettings CreateDynamicDevice(string protocol,string name, Guid id);
    }

    public interface IProfileUpgradeWorker
    {
        bool UpgradeProfile(IApplication applicationProxy, IDeviceProfile deviceProfile);
        void CleanupProfile(IApplication applicationProxy, IDeviceProfile deviceProfile);
    }

    public interface IExtensionLateArrival
    {
        void InitializeLateArrival();
    }

    public interface IExtensionPanel
    {
        string Name { get; }
        string PanelName { get; }
        int SubPanelID { get; }
        int DefaultSortOrder { get; }

        Guid ID { get; }

        IPanelControl CreateControl();
        IReadOnlyList<string> GetPanelVariables();
    }

    public interface IExtensionDevice : IExtensionPanel
    {
        string VendorID { get; }
        string ProductID { get; }
        Guid GetDeviceUniqueIdentifier();
    }

    public interface IExtensionDeviceUpdateProfile
    {
        string DevicePathNew { get; }
        string DevicePathOld { get; }
        string ProductIdNew { get; }
        string ProductIdOld { get; }
        string VendorIdOld { get; }
        string VendoridNew { get; }
        int DeviceEventIndexNew { get; }
    }

    public interface IExtensionGeneric
    {
        GenericSettings Settings { get; }
    }

    public interface IExtensionDevice2 : IExtensionDevice
    {
        bool AutoRemoveInvalidEvents { get; }
        //string DeviceConfigDirectory { get; }
        //string DeviceCalibrationName { get; }
        string DevicePath { get; }
        string DevicePublishName { get; }
    }
    
    public interface IExtensionDynamic 
    {
        bool IsExclusive { get; }
        bool HasDeviceConfiguration { get; }
        
        bool WillHandle(IUSBDevice device);

        bool WillHandle(IDeviceProfile deviceProfile);
    }

    public interface IExtensionDynamicEnable
    {
        bool IsEnabled(IApplication app,IUSBDevice device);
    }

    public interface IExtensionDeviceGroup
    {
        string DeviceGroupName { get; }
        string DeviceGroupResouceKey { get; }
        string DeviceGroupRootPath { get; }
    }

    public interface IExtensionInfo
    {
        string Name { get; }        

        string ProfileOptionsRootPath { get; }
        string ResourceKeyRoot { get;  }
        // TODO: SPAD.neXt.Pages.SettingsPage.additionalSettings.Add(new neXt.Pages.SettingsLink { Name = "Saitek Switchpanel", Page = "/Pages/Settings/ConfigDeviceSettings.xaml#k=CFGSAITEKSWITCHPANEL;o=Saitek.SwitchPanel." });
        bool HasProfileOptions { get;  }
        int ProfileOptionsOrder { get; }
        IReadOnlyList<IExtensionProfileOption> ProfileOptions { get; }
        IReadOnlyList<IExtensionDevice> Devices { get; }
        IReadOnlyList<IExtensionPanel> Panels { get; }
    }
}
