using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPAD.neXt.Interfaces
{
    public interface IPanelHost : IDisposable
    {
        IPanelDevice DeviceAttached { get; }
        IDeviceConfiguration DeviceConfiguration { get; set; }
        IDeviceProfile DeviceProfile { get; }

        IPanelControl PanelControl { get; }
        Guid PanelLinkID { get; }
        Guid DeviceGlobalIdentifier { get; }
        bool PanelHasFocus { get; }
        bool HasPageSupport { get;  }

        string PanelName { get; }
        string DevicePanelID { get; }
        string VendorID { get; }
        string ProductID { get; }
        string DevicePath { get; }
        bool SupportsDefaultProfiles { get; set; }
        bool IsPanelCompletelyInitialized { get; }
        string PanelNavigationFragment { get; }
        bool ShowDialog(string dialogName, ISPADBaseEvent evt, EventHandler configHandler = null);

        string RegisterPanelVariable(string subCategory, string varBaseName, string mainCategory);
        void RegisterPanelVariables(string subCategory, IReadOnlyList<string> vars,string mainCategory = null);
        void UpdatePanelVariable(string name, object value);
        object GetPanelVariable(string name);
        string RegisterDeviceVariable(string varName, object defaultValue, bool overwriteValue = true, bool asReadOnly = true);
        void NavigateToDeviceSettings();
        void DevicePowerChanged(DEVICEPOWER newPowerState);
        void AddPagingSupport();
        void HidePagingSupport();
        void AddPanelButton(UserControl button, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST);
        UserControl AddDropDownCommand(string buttonName, string buttonTag, ICommand command,string mainbuttonLabel = null);
        Button AddPanelButton(string buttonName, string buttonTag, ICommand command, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST, string visibilityProperty = null);
        void AddPanelCheckbox(string buttonName, string buttonTag, ICommand command, bool bIsChecked, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST);
        void SetEventContext(string eventName, Point targetPoint, IInput input);
        IEventContext GetCurrentEventContext();
        string GetConfigurationSetting(string key);
        void NavigateToThis();
        void ClipboardSetData(string data);
        void RenamePage(string newName);
        void RemoveThisPanel();

        void LoadDeviceCalibration(ICalibrateableDevice calibrateableDevice);
        void UpdateEventContextTargetPoint(Point targetPoint);


    }

    public interface IPublishCustomize
    {
        string CustomizePublishname(string defaultName);
    }

    public interface IPanelService
    {

    }

    public interface IPanelImageService : IPanelService
    {
        ImageSource GetImage(string id);
    }

    public interface IPanelControl : IDisposable
    {
        event EventHandler<IPanelDeviceEventArgs> EmulatedDeviceReportReceived;
        Guid UIPanelIdentifier { get; }
        void OnEmulateDeviceReport(IPanelDeviceEventArgs e);

        void InitializePanel(IPanelHost hostControl, string panelLabel);
        bool NeedsAsyncInitialize { get; }
        Task<bool> InitializePanelAsync(CancellationToken cancelToken);
        void DeinitializePanel();
        void SetExtension(IExtensionPanel ctrl);
        void PanelGotFocus();
        void PanelLostFocus();

        void SetApplicationProxy(IApplication appProxy);
        void SetApplicationExtension(IExtension ext);
        void SetSubPanelID(int subPanelID);

        string GetPanelVariableSuffix();
        IReadOnlyList<string> GetPanelVariables();
        bool AllowExternalVariableChange(string varName);
        void MonitoredChanged(string varName, bool isMonitored);
        void DeviceProfileChanged();
        string GetNavigationHint();
        IInput GetAttachedInput(string switchName);

        bool PanelCanRename { get; }
        bool PanelHasSettings { get; }
        bool PanelHasPowerSettings { get; }
        bool IsCommandSupported(string commandName);
        

        void ApplicationReady(BooleanEventArgs e);

        bool CreateDocumentation(IPanelDocumentation docProxy);
        bool InterceptCommand(ICommand command);
        bool SavePanelImage(string filename);

        T GetService<T>(string id = null) where T : class, IPanelService;
        void RaiseEvent(string switchName, ISPADEventArgs eArgs);

        void LoadDeviceCalibration(ICalibrateableDevice calibrateableDevice);

    }

    public interface IPanelDocumentation
    {
        void AddPageBreak();
        void AddSection(string heading);
        void AddParagraph(string text, string style = null);
        void AddImage(string filename);
        void AddEventDocumentation(ISPADBaseEvent evt);
    }

    public interface IProfileEventProvider 
    {
        PanelOptions PanelOptions {get;}
        ISPADBaseEvent FindEvent(string bound);
        void AddEvent(ISPADBaseEvent evt, bool permanent = true);
        bool IsValidEvent(string eventName);
        void RemoveAllEvents();
        bool RemoveEvent(string bound);
        IEnumerable<KeyValuePair<string, string>> GetValidEventChoices(string commandName);
        bool IsCommandSupported(string commandName);

        void PreparePageForExport(IDeviceProfile deviceProfile, IDevicePage devicePage);
        bool PageImported(IDeviceProfile deviceProfile, IDevicePage devicePage);
        Dictionary<string, string> GetPublishInformation();        
    }

    public interface IDevicePage : IExtensible
    {
        Guid ID { get;  }
        bool IsDefaultPage { get; set; }
        string PageName { get; }
        IReadOnlyList<ISPADBaseEvent> Events { get; }
        IReadOnlyList<IDeviceImage> Images { get; }
        void SetID(Guid newGuid);
        void AddEvent(IDeviceProfile profile, ISPADBaseEvent evtIn);
        bool AddUpgradedEvent(ISPADBaseEvent evtIn);

        bool AddImage(IDeviceImage image);
        bool RemoveImage(Guid id);
        ISPADBaseEvent FindEvent(string bound);
        void RemoveAllEvents(IDeviceProfile profile);
        bool RemoveEvent(IDeviceProfile profile, string bound);

        void PageDeactivated(IDeviceProfile profile);
        void PageActivated(IDeviceProfile profile);

        void PageSaveStateVariable(string varName, object value);
        T PageRestoreStateVariable<T>(string varName, T defaultValue = default(T));
    }

    public interface IProfilePageEventProvider
    {
        ISPADBaseEvent FindEvent(string bound);
        void AddEvent(ISPADBaseEvent evt);
        bool IsValidEvent(string eventName);
        void RemoveAllEvents();
        bool RemoveEvent(string bound);
    }
}
