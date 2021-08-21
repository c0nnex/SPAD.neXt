
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
        IApplicationConfiguration CreateConfiguration(string ctrl);
        Guid ID { get; }
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
      
        bool HasDeviceConfiguration { get; }
        
        bool WillHandle(IUSBDevice device);

        bool WillHandle(IDeviceProfile deviceProfile);
    }

    public interface IExtensionDynamicEnable
    {
        bool IsEnabled(IApplication app,IUSBDevice device);
    }

    public interface IExtensionInfo
    {
        string Name { get; }
        string Author { get; }
        string Url { get; }
        string Email { get;  }
        string License { get; }
        string Version { get; }
        string UpdateUrl { get; }

        string ProfileOptionsRootPath { get; }
        string ResourceKeyRoot { get;  }
        // TODO: SPAD.neXt.Pages.SettingsPage.additionalSettings.Add(new neXt.Pages.SettingsLink { Name = "Saitek Switchpanel", Page = "/Pages/Settings/ConfigDeviceSettings.xaml#k=CFGSAITEKSWITCHPANEL;o=Saitek.SwitchPanel." });
        bool HasProfileOptions { get;  }

        IReadOnlyList<IExtensionProfileOption> ProfileOptions { get; }
        IReadOnlyList<IExtensionDevice> Devices { get; }
        IReadOnlyList<IExtensionPanel> Panels { get; }
    }
}
