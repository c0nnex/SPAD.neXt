using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces.Profile
{
    


    public interface IDeviceProfile : IOptionsProvider, IProfileEventProvider, IDisposable, IExtensible, IObservableProvider<ISPADEventArgs>
    {
        string VendorID { get; }
        string ProductID { get; }
        string DevicePath { get; }
        string DeviceID { get; }
        string DeviceTypeID { get; }
        int DeviceIndex { get; }
        string DeviceProfileID { get; }
        string DeviceSerial { get; }
        bool DeviceActive { get; }
        int Version { get; }
        Guid DeviceGlobalIdentifier { get; }

        IDeviceConfiguration DeviceConfiguration { get; }       
        IProfileEventProvider ProfileEventProvider { get; }
        IInputDevice InputDevice { get; }
        string DeviceName { get; }
        string LoggerName { get; }

        event EventHandler DeviceActivated;
        event EventHandler DeviceDeactivated;

        event PropertyChangedEventHandler OptionChanged;

        void ActivateDevice(IPanelHost panelHost);
        void DeactivateDevice();

        ISPADBaseEvent CreateEvent(string tag);
        ISPADBaseEvent CreateEvent(string tag, string xmldata);

        void DeviceConfigurationChanged(IDeviceConfiguration newConfig);
        bool UpdateEventConfiguration(ISPADBaseEvent evt);
        void SetDeviceSerial(string serial);
        void SetProfileEventProvider(IProfileEventProvider provider);
        /* Pages Stuff */
        event EventHandler<IDeviceProfile, IDevicePage> PageAdded;
        event EventHandler<IDeviceProfile, IDevicePage> PageRemoved;
        event EventHandler<IDeviceProfile, IDevicePage> PageActivated;
        event EventHandler<IDeviceProfile, IDevicePage> PageDeactivated;
        event EventHandler<IDeviceProfile, IDevicePage> PageChanged;
        int CountPages { get; }
        Guid ActivePage { get; }
        IReadOnlyList<IDevicePage> PageList { get; }
        IDevicePage CurrentPage { get; }

        void ResetPageEvents();
        IDevicePage SwitchToPage(int pageNum, bool isRelative = false);
        IDevicePage SwitchToPage(Guid pageId);

        IDevicePage CreatePage(string pageName, Guid? id = null);

        bool HasActivationEvents { get; }
        void RegisterActivationEvent(string eventName);
        void UnregisterActivationEvent(string eventName);
        void ExecuteActivationEvents(bool isPageSwitch = false);

        // Image Stuff
        IReadOnlyList<IDeviceImage> GetDeviceImages();
        bool HasDeviceImage(Guid id);
        IDeviceImage GetDeviceImage(Guid id);
        IDeviceImage GetOrCreateDeviceImage(Guid id);
        IDeviceImage ImportDeviceImage(Guid id, string name, byte[] imageData);
        IDeviceImage ImportLocalDeviceImage(string name, string source);
        void ReplaceDeviceImages(Dictionary<Guid, Guid> toReplace);
        void AddDeviceImage(IDeviceImage image);
    }
   
    public interface IImageInfo
    {
        Guid Id { get; }
        string Name { get; }
        string DisplayName { get; }
        long LastChangedTimeStamp { get; }
        DateTime LastChanged { get; }
        string Category { get ; }        
        string Author { get; } 
        string Hash { get; }
        int Status { get; }
    }

    public interface IDeviceImage : ICloneable<IDeviceImage>
    {
        Guid Id { get; }
        string ImageID { get; }
        long LastChangedTimeStamp { get; }
        string Name { get; }
        string DisplayName { get; }
        string Source { get;  }
        bool IsSystemImage { get; }
        bool IsRef { get; }
        void AddReference();
        byte[] GetImage();
        Task<byte[]> GetImageAsync();
        void Refresh();
    }

}
