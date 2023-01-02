using SPAD.neXt.Interfaces.Aircraft;
using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Callout;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Gauges;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using SPAD.neXt.Interfaces.SimConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{

    public interface INoImplicitDispose { }
    public interface ISPAD { }
    public interface IDataMonitorValue : IDisposable
    {
        event EventHandler<IDataMonitorValue, object, object> DataValueChanged; // IDataMonitoValue,newValue,OldValue
        string ID { get; }
        string Name { get; }
        int NumChanges { get; }
        DateTime LastChange { get; }
        object Value { get;  }
        void SetValue(object newValue);
    }

    public static class ServiceEvents
    {
        public static readonly ISPADEventArgs ApplicationStart = new SPADEventArgs("ServiceEvent", "ApplicationReady", 0, 1);
        public static readonly ISPADEventArgs ApplicationStop = new SPADEventArgs("ServiceEvent", "ApplicationReady", 1, 0);
        public static readonly ISPADEventArgs SimulationConnected = new SPADEventArgs("ServiceEvent", "SimulationConnected");
        public static readonly ISPADEventArgs SimulationDisconnected = new SPADEventArgs("ServiceEvent", "SimulationConnected");
        public static readonly ISPADEventArgs Pause = new SPADEventArgs("ServiceEvent", "Pause");
        public static readonly ISPADEventArgs Continue = new SPADEventArgs("ServiceEvent", "Pause");

    }


    public interface IServiceSingleton
    {
        Guid ServiceID { get; }
        Type CreateService();
        void StartService();
        void StartServiceFinal();
        void StopService();
        void StopServiceFinal();
        bool IsEnabled { get; }

        void OnServiceEvent(ISPADEventArgs e);
    }

    /// <summary>
    /// Service will not be started automatically but on demand
    /// </summary>
    public interface IServiceSingletonNoStartup { }

    /// <summary>
    /// Service depends on other services started and will be started after all normal services are running.
    /// </summary>
    public interface IServiceSingletonDelayed { }

    [Serializable]
    public sealed class ImageServicePayload
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        DateTime LastChange { get; set; }

        private string _Hash = null;
        public string Hash
        {
            get
            {
                if (_Hash == null)
                {
                    if (Data != null)
                    {
                        using (var md5 = MD5.Create())
                        {
                            _Hash = BitConverter.ToString(md5.ComputeHash(Data)).Replace("-", "").ToLower();
                        }
                    }
                    else
                        _Hash = "invalid";
                }
                return _Hash;
            }
        }
        public byte[] Data { get; set; }
    }

    public interface IDialogService
    {
        bool ShowDialog(Guid dialogId);
        bool ShowDialog(Guid dialogId, Dictionary<string, object> dialogParameters);
        bool ShowDialog(Guid dialogId, Dictionary<string, object> dialogParameters, out object result);
    }

    public enum ImageCategory
    {
        Inline,
        Device,
        System
    }
    public interface IImageService
    {
        void PreCacheImages(Guid requestId, IEnumerable<IDeviceImage> images);
        bool IsPreCacheCompleted(Guid requestId);

        Task<bool> PutImageAsync(Guid id, string name, byte[] data, string category = null, string subCategory = null);
        Task<bool> PutImagesAsync(IEnumerable<ImageServicePayload> payLoad);
        Task<byte[]> GetImageAsync(Guid id,long lastChange = 0);
        Task<string> GetImageLocalUrlAsync(Guid id, long lastChange = 0);
        bool HasImage(Guid id, string hash = null);
        
        IImageInfo GetImageInformation(Guid id);
        IImageInfo GetImageInformation(string hash);
        IEnumerable<IImageInfo> GetImages(ImageCategory imageCategory = ImageCategory.Inline, Func<IImageInfo,bool> predicate = null);
        IEnumerable<string> GetImageCategories(ImageCategory imageCategory = ImageCategory.Inline);
        Task RefreshAsync();
    }

    public interface ICacheService
    {
        IEnumerable<IDataMonitorValue> GetDataCache(string cacheId);
    }

    public interface IWebVirtualDeviceService
    {
        bool IsEnabled { get; }
        IWebVirtualDevice VirtualDeviceRegister(string deviceTag, string deviceName, string deviceType, string deviceIndexPage, object deviceConfigObject = null);
    }
    public interface IWebVirtualDevice
    {
        event EventHandler<bool> DeviceNeedsUpdate;
        object DeviceConfig { get; }
        string DeviceIndexPage { get; }
        string DeviceName { get; }
        string DeviceTag { get; }
        string DeviceType { get; }
        bool IsEnabled { get; }
        bool IsInUse { get; }
        void Disable();
        void Enable();

        void UpdateConfig(object newConfig, bool notifyClient = true);
        void UpdateData(string dataTag, object data, bool notifyClients = true);
        void UpdateImage(string imageTag, byte[] image, string imageType = "image/bmp", bool notifyClients = true);
    }
    public interface IApplication : ILocalizable, IProfileManager, IEventManager, ICalloutManager, IActionManager, ICacheManager
    {
        Guid ConsumerID { get; }
        bool DebugMode { get; }
        CultureInfo DefaultOSCulture { get; }

        /// <summary>
        /// Currently loaded Aircraft
        /// NULL if none loaded
        /// </summary>
        IAircraft CurrentAircraft { get; }

        bool IsLicenseValid { get; }
        bool IsFeatureLicensed(string feature);
        string ApplicationVersion { get; }
        bool ProgrammingMode { get; }
        void FatalError();
        string GetLicenseID(string feature);
        bool CheckRegisteredFeature();
        ulong GetAuthorID();
        bool CheckDeviceAuthor(string value);
        void RegisterApplicationReady(EventHandler<BooleanEventArgs> applicationReady);
        void RegisterSimulationConnected(EventHandler<SimulationConfiguration, IValueProvider> simulationConnected);
        void RegisterNonDeleteableAction(Guid id);

        T GetService<T>() where T : class;
        T GetService<T>(Guid serviceId);
        // SimConnect Special
        //TODO: Remove FSUIPC add General
        ISimConnectDynamicObject GetSimConnectDataObject(string id, bool clear);
        IReadOnlyList<ISimConnectDynamicObject> GetSimConnectDataObjects();
        void UpdateSimConnectDataObject(ISimConnectDynamicObject simObject);
        void ClearSimConnectDataObject(string id);

        IDataDefinition BrowseDataDefiniton(string curOffset, string titleName, string rootName, bool bWriteOperation, object parentWindow = null);
        IReadOnlyList<IDataDefinition> BrowseDataDefinitonMulti(string curOffset, string titleName, string rootName, bool bWriteOperation, object parentWindow = null);
        IReadOnlyList<IDataDefinition> GetDataDefinitionsByProvider(string provider);

        ISPADBackgroundThread CreateBackgroundThread(string name, ISPADBackgroundWorker worker);
        SPADLogLevel MinLogLevel { get; set; }
        ILogger GetLogger(string name);
        void SendSimulationControl(string ctrlName, uint paramter);

        ISettingsProvider Settings { get; }
        void ApplicationBusy();

        // Gauge Stuff
        IArchive OpenArchive(string archivename);
        IOnlineGaugeList GetGauges(DateTime limes);
        IOnlineGaugeData DownloadGauge(Guid id);
        IGaugeVersionInformation GetGaugeVersionInformation(Guid gaugeId);
        GaugeVersionStatus GetGaugeVersionStatus(Guid gaugeId, Version localVersion);

        void AddResource(string key, string value);
        IDeviceSwitchConfiguration GetSwitchConfiguration(string key);
        string GetFilename(APPLICATION_DIRECTORY scope, string filename);
        string GetDirectory(APPLICATION_DIRECTORY scope, string subDir = null);
        IDirectoryModel DirectoryModel { get; }

        void WarnWithNotification(string LoggerName, string message, params object[] args);
        void ErrorWithNotification(string LoggerName, string message, params object[] args);

        bool RegisterValueProvider(string tag, IValueProvider provider);
        bool RegisterExternalValueProvider(IValueProvider provider);
        bool UnregisterValueProvider(string tag, IValueProvider provider);
        IEnumerable<IValueProviderInfomation> GetRegisteredValueProviders();
        IValueProvider GetValueProvider(string tag);

        IDataDefinition CreateDataDefinition(string provider, string name, string key, string access, string normalizer, string description, string category, string subcategory, bool selectable, double correctionFactor, float espilon);
        IDataDefinition CreateDataDefinition(IValueProvider provider, string name, string globalName);
        IDataDefinition CreateControlDefinition(IValueProvider provider, string name, string globalName);
        void ClearDataDefinitions(IValueProvider provider);
        IDataDefinition RemoveDataDefinition(IValueProvider provider, string id);

        IDataDefinitions CreateDataDefinitions(SPADDefinitionTypes id, StreamReader inputReader);
        void AddDataDefinition(SPADDefinitionTypes definitionType, IDataDefinition item, bool batch = false);
        void AddDataDefinitions(IDataDefinitions data);

        void Broadcast(string eventName, string eventTrigger = null, object eventValue = null);

        void RegisterProfileExtension(string name, Type type);
        ISimulationController GetSimulationController(Guid controllerId);
        ISimulationEventProvider GetSimulationEventProvider();

        void RegisterClientEventProvider(string name, IEventProvider provider);

        IValueTranscriber GetValueTranscriber(string providerName);
        void RegisterSimulationInterface(string name, ISimulationInterface simInterface);

        Version GetRequiredPluginVersion(string pluginName);
        ITransparentValueProvider GetTransparentValueProvider(string providerName);

        IValueProvider GetActiveValueProvider();

        void SetValueProviderStatus(IValueProvider newProvider, bool isActive);
        void OnSimulationConnected(SimulationConfiguration simConfig, IValueProvider provider);

        Stream GetConfigurationFile(string filename, string cfgFile);
        bool HasConfigurationFile(string filename, string cfgFile);
        T ReadXMLConfigurationFile<T>(string filename, string cfgFile) where T : class, new();
        T ReadJSONConfigurationFile<T>(string filename, string cfgFile) where T : class, new();
        T LoadXMLSettingsFile<T>(string filename) where T : class;
        void SaveXMLSettingsFile<T>(string filename, T dataObject) where T : class;

        string CreateXml(object o);
        IReadOnlyList<string> GetJSONConfigurationFiles(string pattern, string cfgFile, bool preferLocal = false);
        IReadOnlyList<IDeviceSwitch> GetDefaultSwitchConfigurations();
        HashSet<string> GetConfigurationSet(string name);
        T GetApplicationOption<T>(string optionKey, T defaultVal = default);

        IExternalExpression CreateExpression(string name, string expression);

        IDynamicNCalcExpression CreateDynamicCalcExpression(string expression);
        bool IsBuild(string buildName);
        IApplicationConfiguration GetApplicationConfiguration(Guid id, string className);
        void RaiseOn(string targetDevice, string targetSwitch, SPADEventArgs eArgs);

        object GetNamedObject(Guid id);
        void RegisterNamedObject(Guid id, object obj, bool asStatic = false);
        void RegisterPanelName(string deviceid, string name, string vendorID, string productID, int eventIndex, int subPanelID);
        void CreateDynamicPanel(string url);

        IRuntimeResolver CreateRuntimeResolver(string id);
        IJoystickEmulator CreateVirtualJoystick(int identifier, string name, string vendorId, string productId);

        ISimpleGauge CreateGauge(bool noLayers);
    }

    public interface IActionManager
    {
        void RegisterActionProvider(IActionProvider provider);
        IReadOnlyList<IActionProvider> GetRegisteredActionProviders();

    }

    public interface ICacheManager
    {
        IDataCacheValueProvider CreateDisplayCache(string id);
    }

    public interface IApplicationConfiguration2 : IApplicationConfiguration
    {
        void SetContext(string key, string context);
    }

    public interface IApplicationConfiguration
    {
        bool ExecuteConfiguration(IApplication app);

    }

    public interface IDeviceProfileEventConverter : IDisposable
    {
        bool ConvertEvent(ILogger logger, ISPADBaseEvent evt, IDeviceProfile deviceProfile, IDeviceConfiguration deviceConfiguration, IDeviceSwitch deviceSwitch, int xmlVersion, string converterParameter);
    }

    public interface IRuntimeResolver
    {
        bool HasFile(string filename);
        Stream GetStream(string filename = null);
        StreamReader GetStreamReader(string filename = null);
        string GetContent(string filename = null);

        void Warning(string message);
    }

    public interface IResolvesAtRuntime
    {
        void RuntimeResolve(IApplication proxy, IRuntimeResolver resolver);
    }

    public interface ISupportsActivation
    {
        void Activate(string source, object additionalInfo);
        void Deactivate(string source, object additionalInfo);

        void Rotate(int direction, string source, object additionalInfo);
    }

    public interface IMonitorWindow : IApplicationConfiguration
    {
        void Show();
        void Setup(IApplication proxy, IEnumerable<IDataMonitorValue> profVals, string placementName = null);
    }

    public interface IDeviceEmulation
    {
        void EmulateClientConnect(object data);
    }

    public interface IJoystickEmulator : ICalibrateableDevice, IInputDevice
    {
        IInputAxis CreateAxis(string name, uint id, int min, int max);
    }
}

