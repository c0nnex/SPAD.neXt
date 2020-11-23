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
    public interface IDeviceProfile : IOptionsProvider, IProfileEventProvider,IDisposable, IObservableProvider<ISPADEventArgs>
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

        IReadOnlyList<ISPADBaseEvent> Events { get; }

        event EventHandler DeviceActivated;
        event EventHandler DeviceDeactivated;
     
        event PropertyChangedEventHandler OptionChanged;

        void ActivateDevice();
        void DeactivateDevice();

        ISPADBaseEvent CreateEvent(string tag);
        ISPADBaseEvent CreateEvent(string tag, string xmldata);

        bool UpdateEventConfiguration(bool ignoreNoAutoRemove);
        bool UpdateEventConfiguration(ISPADBaseEvent evt);
        void UpdateEventTarget(ISPADBaseEvent evt);
        void SetDeviceSerial(string serial);
        T GetExtension<T>(Type type) where T : IXmlAnyObject;
        bool HasExtension<T>() where T : IXmlAnyObject;
    }
}
