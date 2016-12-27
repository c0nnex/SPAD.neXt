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
using System.Globalization;
using System.IO;

namespace SPAD.neXt.Interfaces
{
    public interface IApplication : ILocalizable, IProfileManager, IEventManager, ICalloutManager
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
        void FatalError();
        string GetLicenseID(string feature);
        bool CheckRegisteredFeature();
        UInt32 GetAuthorID();
        void RegisterApplicationReady(EventHandler<BooleanEventArgs> applicationReady);
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
        void SendSimulationControl(string ctrlName, int paramter);

        ISettingsProvider Settings { get; }
        void ApplicationBusy();

        // Gauge Stuff
        IArchive OpenArchive(string archivename);
        IReadOnlyList<IOnlineGauge> GetGauges(DateTime limes);
        IOnlineGaugeData DownloadGauge(string id);

        void AddResource(string key, string value);
        IDeviceSwitchConfiguration GetSwitchConfiguration(string key);
        string GetFilename(APPLICATION_DIRECTORY scope, string filename);
        string GetDirectory(APPLICATION_DIRECTORY scope, string subDir = null);
        IDirectoryModel DirectoryModel { get; }

        void WarnWithNotification(string LoggerName, string message, params object[] args);
        void ErrorWithNotification(string LoggerName, string message, params object[] args);

        bool RegisterValueProvider(string tag, IValueProvider provider);
        bool RegisterExternalValueProvider(IValueProvider provider);
        bool UnregisterValueProvider(IValueProvider provider);
        IEnumerable<IValueProviderInfomation> GetRegisteredValueProviders();
        IValueProvider GetValueProvider(string tag);

        IDataDefinition CreateDataDefinition(string provider, string name, string key, string access, string normalizer, string description, string category, string subcategory, bool selectable, double correctionFactor, float espilon);
        IDataDefinition CreateDataDefinition(IValueProvider provider, string name, string globalName);
        IDataDefinition CreateControlDefinition(IValueProvider provider, string name, string globalName);

        IDataDefinitions CreateDataDefinitions(SPADDefinitionTypes id, StreamReader inputReader);
        void AddDataDefinition(SPADDefinitionTypes definitionType, IDataDefinition item, bool batch = false);
        void AddDataDefinitions(IDataDefinitions data);

        void Broadcast(string eventName, string eventTrigger = null, object eventValue = null);

        void RegisterProfileExtension(string name, Type type);
        ISimulationController GetSimulationController(Guid controllerId);

        void RegisterClientEventProvider(string name, IEventProvider provider);

        IValueTranscriber GetValueTranscriber(string providerName);
        void RegisterSimulationInterface(string name, ISimulationInterface simInterface);

        Version GetRequiredPluginVersion(string pluginName);
    }

    public interface IApplicationConfiguration
    {
        bool ExecuteConfiguration(IApplication app);
    }


}
