using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPAD.neXt.Interfaces
{
    public interface IPanelHost
    {
        IPanelDevice DeviceAttached { get; }
        IDeviceConfiguration DeviceConfiguration { get; }
        IDeviceProfile DeviceProfile { get; }
        IProfileEventProvider ProfileEventProvider { get; }

        IPanelControl PanelControl { get; }
        Guid PanelLinkID { get; }
        bool PanelHasFocus { get; }
        bool IsVirtualDevice { get; }
        bool HasPageSupport { get;  }

        string PanelName { get; }
        string DevicePanelID { get; }
        string VendorID { get; }
        string ProductID { get; }
        string DevicePath { get; }
        bool SupportsDefaultProfiles { get; set; }
        bool ShowDialog(string dialogName, ISPADBaseEvent evt, EventHandler configHandler = null);

        void RegisterPanelVariables(string subCategory, IReadOnlyList<string> vars,string mainCategory = null);
        void UpdatePanelVariable(string name, object value);
        object GetPanelVariable(string name);

        void NavigateToDeviceSettings();
        void DevicePowerChanged(DEVICEPOWER newPowerState);
        void AddPagingSupport();
        void AddPanelButton(UserControl button, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST);
        UserControl AddDropDownCommand(string buttonName, string buttonTag, ICommand command,string mainbuttonLabel = null);
        Button AddPanelButton(string buttonName, string buttonTag, ICommand command, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST);
        void AddPanelCheckbox(string buttonName, string buttonTag, ICommand command, bool bIsChecked, PANEL_BUTTONPOSITION position = PANEL_BUTTONPOSITION.LAST);
        void SetEventContext(string eventName, Point targetPoint, IInput input);
        IEventContext GetCurrentEventContext();
        string GetConfigurationSetting(string key);
        void NavigateToThis();
        void ClipboardSetData(string data);
        void RenamePage(string newName);
        void RemoveThisPanel();

        void LoadDeviceCalibration(ICalibrateableDevice calibrateableDevice);
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
        
        void OnEmulateDeviceReport(IPanelDeviceEventArgs e);

        void InitializePanel(IPanelHost hostControl, string panelLabel);
        bool NeedsAsyncInitialize { get; }
        Task<bool> InitializePanelAsync();
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
        IProfileEventProvider ProfileEventProvider { get; }

        void ApplicationReady(BooleanEventArgs e);

        bool CreateDocumentation(IPanelDocumentation docProxy);
        bool InterceptCommand(ICommand command);
        bool SavePanelImage(string filename);

        T GetService<T>(string id = null) where T : class, IPanelService;
        void RaiseEvent(string switchName, ISPADEventArgs eArgs);
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
        void AddPage(IDevicePage page);
        
        void RemovePage(IDevicePage page);
        IDevicePage FindPage(Guid id);
        IDevicePage GetDefaultPage();
        bool IsValidEvent(string eventName);
        void RemoveAllEvents();
        void RemoveAllPages();
        bool RemoveEvent(string bound);
        IEnumerable<KeyValuePair<string, string>> GetValidEventChoices(string commandName);
        bool IsCommandSupported(string commandName);

        
    }

    public interface IDevicePage : IExtensible
    {
        Guid ID { get;  }
        bool IsDefaultPage { get; set; }
        string PageName { get; }
        IReadOnlyList<ISPADBaseEvent> Events { get; }

        void SetID(Guid newGuid);
        void AddEvent(IDeviceProfile profile, ISPADBaseEvent evtIn);
        bool AddUpgradedEvent(ISPADBaseEvent evtIn);

        ISPADBaseEvent FindEvent(string bound);
        void RemoveAllEvents(IDeviceProfile profile);
        bool RemoveEvent(IDeviceProfile profile, string bound);

        void PageDeactivated(IDeviceProfile profile);
        void PageActivated(IDeviceProfile profile);
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
