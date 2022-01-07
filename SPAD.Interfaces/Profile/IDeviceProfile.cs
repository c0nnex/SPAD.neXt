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

        IDeviceConfiguration DeviceConfiguration { get; }
        IPanelHost PanelHost { get; }

        string DeviceName { get; }
        string LoggerName { get; }

        event EventHandler DeviceActivated;
        event EventHandler DeviceDeactivated;

        event PropertyChangedEventHandler OptionChanged;

        void ActivateDevice();
        void DeactivateDevice();

        ISPADBaseEvent CreateEvent(string tag);
        ISPADBaseEvent CreateEvent(string tag, string xmldata);

        bool UpdateEventConfiguration(bool ignoreNoAutoRemove);
        bool UpdateEventConfiguration(ISPADBaseEvent evt);
        void SetDeviceSerial(string serial);

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
    }
}
