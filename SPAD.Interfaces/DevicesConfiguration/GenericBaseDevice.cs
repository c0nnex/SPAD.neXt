using SPAD.Extensions.Generic;
using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Extension;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Extension.Generic
{
    public abstract class PanelExtensionBase<T> : IExtension, IExtensionDynamicDevice where T : class
    {
        public static IApplication ApplicationProxyStatic { get; private set; } = null;
        public static T Instance { get; private set; }
        public IApplication ApplicationProxy { get; private set; }
        private List<IPanelControl> _AllControls = new List<IPanelControl>();
        protected ILogger logger { get; set; }

        public void StartupExtension(IApplication app)
        {
            this.ApplicationProxy = app;
            ApplicationProxyStatic = this.ApplicationProxy;
            Instance = this as T;
            logger = app.GetLogger(this.GetType().FullName);
            OnStartupExtenion();
        }

        public virtual void OnStartupExtenion() { }
        public virtual void OnShutdownExtenion() { }
        public abstract IExtensionInfo GetExtensionInformation();

        public void ShutdownExtenion()
        {
            foreach (var item in _AllControls)
            {
                item.DeinitializePanel();
            }
            _AllControls.Clear();
            OnShutdownExtenion();
        }
        public abstract string ActivatedSetting { get; }

        public abstract Guid ID { get; }

        public virtual bool IsExtensionEnabled(IApplication app)
        {
            if (String.IsNullOrEmpty(ActivatedSetting))
                return true;
            return app.Settings.GetOption(ActivatedSetting).AsBoolean;
        }
        public IPanelControl CreateControl(IExtensionPanel ctrl)
        {
            IPanelControl ct = ctrl.CreateControl();
            ct.SetExtension(ctrl);
            ct.SetApplicationProxy(ApplicationProxy);
            ct.SetApplicationExtension(this);
            ct.SetSubPanelID(ctrl.SubPanelID);
            _AllControls.Add(ct);
            return ct;
        }
        public void DisposeControl(IPanelControl ctrl)
        {
            _AllControls.Remove(ctrl);
        }
        public virtual void InitializeExtension() { }
        public virtual void DeinitializeExtension() { }
        public virtual IApplicationConfiguration CreateConfiguration(string ctrl) => null;
        public virtual GenericSettings CreateDynamicDevice(string protocol, string name, Guid id) => null;
    }

    public abstract class GenericDeviceBase
    {
        public event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnDeviceEvent;
        protected object lockObject = new object();

        private AddonDevice _AddonDevice;
        public AddonDevice AddonDevice
        {
            get => _AddonDevice;
            protected set
            {
                _AddonDevice = value;
            }
        }
        public IRuntimeResolver RuntimeResolver { get; set; }

        protected void RaiseDeviceEvent(ISPADEventArgs eventArgs)
        {
            OnDeviceEvent?.Invoke(this as IGenericCommandDevice, eventArgs);
        }

        public virtual void DeviceEnabled()
        {
        }

        public virtual void DeviceDisabled()
        {
        }

        public virtual void DeviceActivated(IDeviceProfile device) { }
        public virtual void DeviceDeactivated() { }

        public virtual void DeviceExecuteEvent(ISPADEventArgs eventArgs) { }

        public virtual void UpdateImage(string tag, byte[] imageData) { }


    }

    public abstract class GenericDevice : GenericDeviceBase, IGenericCommandDevice
    {
        public enum DeviceStates
        {
            Error,
            WaitForConnection,
            WaitForDeviceId,
            WaitForDeviceVersion,
            Running,
            Syncing
        }

        private bool disposedValue;
        protected GenericSettings Settings;
        protected IApplication ApplicationProxy;
        protected ILogger logger;
        protected IGenericCommandDeviceCallback PanelCallback;
        public string DeviceVendor { get; protected set; }
        public string DeviceName { get; protected set; }
        public string DeviceIdentifier { get; protected set; }
        public string DeviceSerial { get; protected set; }
        public string DevicePublishName { get; protected set; }
        public Version DeviceVersion { get; protected set; } = new Version(0, 0, 0);
        public DateTime DeviceLastPing { get; protected set; } = DateTime.Now;

        private DeviceStates _CurrentDeviceState = DeviceStates.WaitForConnection;
        public DeviceStates DeviceState
        {
            get => _CurrentDeviceState;
            protected set
            {
                if (_CurrentDeviceState != value)
                {
                    var oldState = _CurrentDeviceState;
                    _CurrentDeviceState = value;
                    Log("State changed to " + value);
                    DeviceStateChanged(oldState, _CurrentDeviceState);
                }
            }
        }

        public Guid DeviceGuid { get; protected set; } = Guid.Empty;
        public bool IsConnected { get; protected set; }
        public bool IsPowered { get; protected set; }
        protected bool IsLicensed = true;
        public event EventHandler<object, string> OnLog;
        public event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnRaiseEvent;
        public event EventHandler<IGenericCommandDevice, ISPADEventArgs> OnRaiseTunerEvent;
        public event EventHandler<IGenericCommandDevice, string> OnCommandReceived;
        public event EventHandler<IGenericCommandDevice, IGenericCommandDevice> OnConfigurationCompleted;
        public event EventHandler<IGenericCommandDevice, string> OnConfigurationFailed;
        public event EventHandler<IGenericCommandDevice, bool> OnConnectionStateChanged;

        protected void RaiseEvent(ISPADEventArgs args) => OnRaiseEvent?.Invoke(this, args);
        protected void RaiseEncoderEvent(ISPADEventArgs args) => OnRaiseTunerEvent?.Invoke(this, args);
        protected void CommandReceived(string cmd) => OnCommandReceived?.Invoke(this, cmd);
        protected virtual void ConfigurationCompleted() => OnConfigurationCompleted?.Invoke(this, this);
        protected void ConnectionStateChanged(bool isConnected)
        {
            IsConnected = isConnected;
            OnConnectionStateChanged?.Invoke(this, isConnected);
        }
        protected void ConfigurationFailed(string message) => OnConfigurationFailed?.Invoke(this, message);

        protected virtual void DeviceStateChanged(DeviceStates oldState, DeviceStates newState) { }

        public abstract Task<bool> Connect();
        public abstract void InitializeDevice(IApplication applicationProxy, GenericSettings settings);

        protected bool IsDebug = false;

        public void SetDebug(bool enable) => IsDebug = enable;
        public void CreateDevice(IApplication applicationProxy, GenericSettings settings)
        {
            Settings = settings;
            ApplicationProxy = applicationProxy;
            IsDebug = ApplicationProxy.GetApplicationOption("Device:serial:Debug", false);
            if (settings.Logger != null)
                logger = settings.Logger;
            else
                logger = ApplicationProxy.GetLogger("Devices.Generic");
            PanelCallback = settings.PanelBaseControl as IGenericCommandDeviceCallback;
            InitializeDevice(applicationProxy, settings);
        }

        public virtual void SendCommand(GenericCommand sendCommand) { }

        public abstract void Disconnect();
        public virtual IInput GetAttachedInput(string name) => null;
        public virtual void AircraftChanged(string newAircraft) { }
        public virtual void LedStatusChanged(string tag, LedStatusEventArgs newState) { }

        public virtual void PageActivated(IDeviceProfile profile, IDevicePage page, bool changeCompleted) { }
        public virtual void ProfileChanged(bool isCompleted) { }
        public virtual void VirtualPowerChanged(bool newPowerState) { IsPowered = newPowerState; }

        protected void Log(string message)
        {
            logger?.Info(message);
        }
        protected void Warn(string message)
        {
            logger?.Warn(message);
        }
        protected void Debug(string message)
        {
            if (IsDebug)
                logger?.Info(message);
            else
                logger?.Debug(message);
        }
        protected void Error(string message)
        {
            logger?.Warn(message);
        }
        protected AddonDevice LoadDeviceConfiguration(IApplication applicationProxy, Action<AddonDevice> beforeFixup = null, bool doFixUp = true)
        {
            try
            {
                AddonDevice addonDevice = null;

                addonDevice = applicationProxy.ReadXMLConfigurationFile<AddonDevice>(System.IO.Path.Combine(DeviceVendor, DeviceName + ".xml"), DataConfiguration.Devices);
                if (addonDevice == null)
                    addonDevice = applicationProxy.ReadXMLConfigurationFile<AddonDevice>(System.IO.Path.Combine(DeviceVendor, DeviceName, "device.xml"), DataConfiguration.Devices);
                if (addonDevice != null)
                {
                    addonDevice.BasePath = System.IO.Path.Combine(DeviceVendor, DeviceName);
                }
                if (addonDevice != null)
                {
                    addonDevice.ProcessImports((item) => applicationProxy.ReadXMLConfigurationFile<AddonDevice>(System.IO.Path.Combine(addonDevice.BasePath, item), DataConfiguration.Devices));
                    if (doFixUp)
                    {
                        if (beforeFixup != null)
                            beforeFixup(addonDevice);
                        addonDevice.FixUp(applicationProxy);
                    }
                }
                return addonDevice;
            }
            catch
            {
#if DEV
                throw;
#endif
                return null;
            }
        }
        protected virtual void OnDispose() { }
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)/OnDispose' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}


