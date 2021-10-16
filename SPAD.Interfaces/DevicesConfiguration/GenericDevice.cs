
using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Extension;
using System;
using System.Threading.Tasks;

namespace SPAD.Extensions.Generic
{
    public class GenericSettings
    {
        public string Protocol { get; set; }
        public string PortName { get; set; }

        public  IPanelControl PanelBaseControl { get; set; }
        public IPanelDevice   AttachedDevice { get; set; }
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
    }

    public class GenericCommand
    {
        public string Command;
        public int WaitAfterSend = 0;

        public GenericCommand(string command, int waitAfterSend = 0)
        {
            Command = command;
            WaitAfterSend = waitAfterSend;
        }
    }

    public interface IGenericLearn
    {
        Task<bool> LearnInput(AddonDeviceElement addonDeviceElement);
    }

    public interface IGenericCommandDevice : IDisposable
    {
        event EventHandler<object, string> OnLog;
        event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnRaiseEvent;
        event EventHandler<IGenericCommandDevice, string> OnCommandReceived;
        event EventHandler<IGenericCommandDevice, IGenericCommandDevice> OnConfigurationCompleted;
        event EventHandler<IGenericCommandDevice, bool> OnConnectionStateChanged;
        string DeviceVendor { get; } // GUID of Vendor (e.g. ShakePrint / RealSimGear , mapped to realname via ressource)
        string DeviceName { get; } // Readable Name of Device
        string DeviceIdentifier { get; } // GUID of Device
        string DeviceSerial { get; }
        string DevicePublishName { get; }
        Version DeviceVersion { get; } // VersionNumber of Device

        DateTime DeviceLastPing { get; } // Last Packet/Pong from Device

        AddonDevice AddonDevice { get; }
        bool IsConnected { get; }

        void Connect();
        void Disconnect();
        void SendCommand(GenericCommand sendCommand);
        void CreateDevice(IApplication applicationProxy, GenericSettings settings);

        // event functions
        void OnProfileChanged();
        void OnAircraftChanged(string newAircraft);
        void OnVirtualPowerChanged(bool newPowerState);

        void OnLedStatusChanged(string tag, bool newState);

    }

}
