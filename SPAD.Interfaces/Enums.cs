using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public enum PanelDevicReadStatus
    {
        Success = 0,
        WaitTimedOut = 1,
        WaitFail = 2,
        NoDataRead = 3,
        ReadError = 4,
        NotConnected = 5
    }

    [Flags]
    public enum SimulationTypes
    {
        UNKNOWN,
        FSX = 1,
        P3Dv3 = 2,
        DCS = 4,
        XPLANE = 8,
        FSX_SE = 16,
        P3Dv2 = 32,
        OTHER3 = 64,
        OTHER4 = 128
    }

    [Flags]
    public enum SPADDefinitionTypes
    {
        OFFSET = 0x1,
        CONTROL = 0x2,
        ALL = 0x3

    }

    public enum SPADEventActions
    {
        UNKNOWN,
        DONOTHING,
        DATA,
        CONTROL,
        AXIS,
        PREDEFINED_AXIS,
        JOYSTICK,
        KEYBOARD,
        PLATECOLOR,
        LEDCOLOR,
        CHANGESTATE,
        SCRIPT,
        DELAY,
        COMMAND,
        BUTTONLIGHT,
        DISPLAY,
        BUTTONMODE,
        PLAYSOUND
    }

    public enum SPADEventValueComparator
    {
        Equals,
        Unequal,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Mask,
        Not,
        AnyBitSet,
        IsBitSet,
        IsBitNotSet,
        Ignore,
        Always,
        Range,
        None
    }

    public enum EventOperations
    {
        Normal,
        Multiply,
        Divide,
        Add,
        Substract,
        MinMax
    }

    public enum SPADValueOperation
    {
        Set,
        Increment,
        Decrement,
        SetBit,
        ClearBit,
        ToggleBit
    }

    public enum SPADSoundOperation
    {
        Play,
        Playloop,
        Stop,
    }

    public enum SPADBooleanValue
    {
        ON = 1,
        OFF = 0
    }

    public enum SPADConditionBinding
    {
        AND,
        OR
    }


    public enum SPADColors
    {
        OFF = 0,
        GREEN = 1,
        YELLOW = 2,
        RED = 4,
        GREENBLINK = 0xF001,
        YELLOWBLINK = 0xF002,
        REDBLINK = 0xF004,
    }

    public enum SPADKeyboardOption
    {
        PRESSANDRELEASE = 0,
        PRESS = 1,
        RELEASE = 2,
        // Macro = 3
    }

    public enum SPADButtonLight
    {
        OFF,
        BUTTON_SHORT_ON,
        BUTTON_SHORT_OFF,
        BUTTON_LONG_ON,
        BUTTON_LONG_OFF,
    }

    public enum JoystickAxis
    {
        AxisX, AxisY, AxisZ, AxisRX, AxisRY, AxisRZ
    }

    public enum JoystickInputTypes
    {
        Button,
        Axis,
        HatSwitchButton,
        VirtualHatSwitch,
        ModeSwitch,
        Unkown
    }

    public enum JoystickHatDirection
    {
        Neutral = 0,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    [Flags]
    public enum PanelOptions
    {
        PanelCopyThis = 1,
        PanelCopyAll = 2,
        PanelCopyDevice = 4,
        PanelPasteThis = 8,
        PanelPasteAll = 16,
        PanelPasteDevice = 32,
        PanelClipboardAll = PanelCopyAll | PanelCopyThis | PanelPasteAll | PanelPasteThis | PanelCopyDevice | PanelPasteDevice,
    }

    public enum GaugePowerMode
    {
        Default,
        AlwaysOn,
        NoData
    }

    [Flags]
    public enum VARIABLE_SCOPE
    {
        SCOPE_SESSION,
        SCOPE_PROFILE,
        SCOPE_GAUGE,
        SCOPE_ANY = VARIABLE_SCOPE.SCOPE_GAUGE | VARIABLE_SCOPE.SCOPE_PROFILE | VARIABLE_SCOPE.SCOPE_SESSION,
    }

    public enum APPLICATION_DIRECTORY
    {
        DIRECTORY_INSTALL,
        DIRECTORY_APPDATA,
        DIRECTORY_DOCUMENTS,
        DIRECTORY_PRIVATE,
        DIRECTORY_CONFIGURATION,
        DIRECTORY_LOG,
        DIRECTORY_GAUGES,
        DIRECTORY_PROFILES,
        DIRECTORY_SCRIPTS,
    }

    public enum PANEL_BUTTONPOSITION
    {
        FIRST = 0,
        LAST = -1
    }

    public enum DEVICEPOWER
    {
        Enable,
        Disable,
        On,
        Off
    }

    public enum CustomExpressionTypes
    {
        Expression = 0,
        Script = 1,
    }
    /// <summary>
    /// Input modifiers.
    /// </summary>
    public enum InputModifier
    {
        /// <summary>
        /// Input is not modified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Input is pulsed once when triggered.
        /// </summary>
        Pulse,

        /// <summary>
        /// Input is pulsed repeatedly while triggered.
        /// </summary>
        Repeat,

        /// <summary>
        /// Input is toggled when triggered.
        /// </summary>
        Toggle,

        /// <summary>
        /// Input is pulsed when triggered and released before the delay elapses.
        /// </summary>
        ShortPress,

        /// <summary>
        /// Input is pulsed when triggered and released after the delay elapses.
        /// </summary>
        LongPress,

        /// <summary>
        /// Input is triggered on long press and released on short press.
        /// </summary>
        LongPressLock,
    }

    public enum XmlSerilizationPurpose
    {
        Normal,
        Export,
        Cloning
    }

    public enum GaugeLicenseModel
    {
        NONE,
        FREE,
        LICENSE,
        PROTECT,
        
    }

    public static class InternalEvents
    {
        public const string UpdateDisplay = "__UPDATEDISPLAY__";
        public const string ALL = "__ALL__";
        public const string UpdateAllDisplays = "__ALL__.__UPDATEDISPLAY__";
        public const string UpdateLeftDisplay = "__UPDATELEFTDISPLAY__";
        public const string UpdateRightDisplay = "__UPDATELEFTDISPLAY__";
        public const string ButtonLight = "__BUTTONLIGHT__";
        public const string ButtonMode = "__BUTTONMODE__";
        public const string LEDColor = "__LEDCOLOR__";
        public const string PlateColor = "__PLATECOLOR__";
        public const string Digitmark = "__DIGITMARK__";
        public const string Display = "DISPLAY";
        public const string LeftDisplay = "LEFTDISPLAY";
        public const string RightDisplay = "RIGHTDISPLAY";
        public const string UpdateLabel = "__UPDATELABEL__";
        public const string UpdateVariable = "__UPDATEVAR__";
        public const string ActiveWindow = "__ACTIVEWINDOW__";
        public const string FSXMainWindow = "FS98MAIN";
        public const string ModeChanged = "__MODECHANGED__";
        public const string ModeReport = "__MODEREPORT__";
    }

    public static class SPADSystemEvents
    {
        public const string AircraftChanged = "SPAD_Aircraft";
        public const string FSUIPCChanged = "FSUIPC.Status";
        public const string SimConnectChanged = "SimConnect.Status";
        public const string CDUAvailable = "SPAD_CDUAvailable";
    }
    public static class OptionNames
    {
        public const string Sensitivity = "SENSITIVITY";
        public const string Name = "NAME";
    }

    public static class LOCALVARIABLE
    {
        public const string SYSTEM_READY = "SYSTEM READY";
        public const string SIMCONNECT_STATUS = "SIMCONNECT STATUS";
        public const string FSUIPC_STATUS = "FSUIPC STATUS";
        public const string LVAR_STATUS = "LVAR STATUS";

    }
    public class Constants
    {
        public const string Namespace = "http://www.fsgs.com/SPAD";
        public const string ConfigNamespace = "http://www.fsgs.com/SPAD/v2";
        public const UInt32 DEFINE_ID_NOTREGISTERED = UInt32.MaxValue;

        public static readonly Guid EVENTACTION_NEWID = Guid.Empty;
        public static readonly Guid EVENTACTION_POWERON_GUID = new Guid("{60F846D4-79F9-48F9-9C7E-59CDFFA5AD9A}");
        public static readonly Guid EVENTACTION_POWEROFF_GUID = new Guid("{BF175192-C786-4526-873F-2362388A9445}");
        public static readonly Guid EVENTACTION_DISPLAY_GUID = new Guid("{F3BD2794-9EB4-4016-ABE2-052989F775A9}");

        public static readonly Guid DEVICE_NOTREGISTERED = new Guid("{A6C592BD-69A8-4F8B-8541-FBF9FB717A7E}");
    }

    public class VendorIDs
    {
        public const string Saitek = "0x06A3";
        public const int iSaitek = 0x06A3;
        public const int iUltimarc = 0xD209;
    }

    public class ProductIDs
    {
        public const string SaitekBIP = "0x0B4E";
        public const string SaitekSwitchPanel = "0x0D67";
        public const string SaitekMultiPanel = "0x0D06";
        public const string SaitekFIP = "0xA2AE";
        public const string SaitekRadioPanel = "0x0D05";
    }
}
