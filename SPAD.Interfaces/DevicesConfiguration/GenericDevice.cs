
using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Extension;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using SPAD.neXt.Interfaces.Transport;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SPAD.Extensions.Generic
{
    
    public class RequireStruct<T> where T : struct { }
    public class RequireClass<T> where T : class { }
    public class GenericSettings
    {
        public Guid ID { get; set; } = Guid.Empty;
        public string Protocol { get; set; }
        public string PortName { get; set; }

        public  IPanelControl PanelBaseControl { get; set; }
        public IPanelDevice   AttachedDevice { get; set; }
        public ITransport Transport { get; set; }
        public ILogger Logger { get; set; }
        public GenericSettings() { }
        public GenericSettings(string portName,string protocol)
        {
            PortName = portName;
            Protocol = protocol;
        }

        public override string ToString()
        {
            return $"Generic {PortName}:{Protocol}";
        }

        private Dictionary<string, object> Options = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        public T GetOption<T>(string key,T defValue = default(T), RequireClass<T> ignoreMe = null) where T : class 
        {
            if (Options.TryGetValue(key, out object value))
                return value as T;
            return defValue as T;
        }
        public T GetOption<T>(string key, T defValue = default,RequireStruct<T> ignoreMe = null) where T : struct
        {
            if (Options.TryGetValue(key, out object value))
                return (T)value;
            return defValue;
        }
        public void SetOption(string key,object value) => Options[key] = value;

        public int PortNumber { get => GetOption("PortNumber", 0); set => SetOption("PortNumber", value); }
        public IntPtr PortInterface { get => GetOption("PortInterface", IntPtr.Zero); set => SetOption("PortInterface", value); }
    }

    public class GenericCommand
    {
        public string Command;
        public string Parameter;
        public object Argument;
        public int WaitAfterSend = 0;
        public bool NoLog { get; set; } = false;
        public GenericCommand(string command, int waitAfterSend = 0, bool noLog = false)
        {
            Command = command;
            WaitAfterSend = waitAfterSend;
            NoLog = noLog;
        }
 
        public GenericCommand(string command,string parameter=null, int waitAfterSend = 0) : this(command,waitAfterSend)
        {
            Parameter = parameter;
        }
    }

    public interface IGenericLearn
    {
        Task<bool> LearnInput(AddonDeviceElement addonDeviceElement);
    }

    public interface  IGenricCreateInteraction
    {
        FrameworkElement CreateInteraction(AddonDeviceElement addonDeviceElement,bool asInactive = false);
    }

    public interface IGenericCommandDeviceCallback
    {
        void SetIsCompleteEditionFeature();
        void RaiseEvent(IGenericCommandDevice device,ISPADEventArgs eventArgs);
        void RaiseEncoderEvent(IGenericCommandDevice device, ISPADEventArgs eventArgs);
        void RaiseAxisEvent(IGenericCommandDevice device, AxisInputEventArgs eventArgs);
    }


    public interface IGenericCommandDevice : IDisposable
    {
        event EventHandler<object, string> OnLog;
        event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnRaiseEvent;
        event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnRaiseTunerEvent;
        event EventHandler<IGenericCommandDevice, string> OnCommandReceived;
        event EventHandler<IGenericCommandDevice, IGenericCommandDevice> OnConfigurationCompleted;
        event EventHandler<IGenericCommandDevice, string> OnConfigurationFailed;
        event EventHandler<IGenericCommandDevice, bool> OnConnectionStateChanged;

        IRuntimeResolver RuntimeResolver { get; }
        string[] GetLogBuffer();

        string DeviceVendor { get; } // GUID of Vendor (e.g. ShakePrint / RealSimGear , mapped to realname via ressource)
        string DeviceName { get; } // Readable Name of Device
        string DeviceIdentifier { get; } // GUID of Device
        string DeviceSerial { get; }
        string DevicePublishName { get; }
        Version DeviceVersion { get; } // VersionNumber of Device

        DateTime DeviceLastPing { get; } // Last Packet/Pong from Device

        AddonDevice AddonDevice { get; }
        bool IsConnected { get; }

        Task<bool> Connect();
        void Disconnect();
        void SendCommand(GenericCommand sendCommand);
        void CreateDevice(IApplication applicationProxy, GenericSettings settings);

        // event functions
        void OnProfileChanged(bool isCompleted);
        void OnAircraftChanged(string newAircraft);
        void OnVirtualPowerChanged(bool newPowerState);

        void OnLedStatusChanged(string tag, LedStatusEventArgs newState);
        void OnPageActivated(IDeviceProfile profile,IDevicePage page, bool changeCompleted);

        IInput GetAttachedInput(string name);
    }

}
