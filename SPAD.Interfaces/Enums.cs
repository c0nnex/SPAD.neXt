using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    [Flags]
    public enum FontStyle
    {
        //
        // Summary:
        //     Normal text.
        Regular = 0,
        //
        // Summary:
        //     Bold text.
        Bold = 1,
        //
        // Summary:
        //     Italic text.
        Italic = 2,
        //
        // Summary:
        //     Underlined text.
        Underline = 4,
        //
        // Summary:
        //     Text with a line through the middle.
        Strikeout = 8
    }

    public enum AxisCurveResponseType
    {
        None,
        Multiplier,
        Values
    }
    public enum AxisCurveShapeType
    {
        Bezier,
        Cardinal
    }

    public enum DisplayFormatting
    {
        RightToLeft = 0,
        LeftToRight = 1
    }

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
        SCS = 64,
        IRACING = 128,
        P3Dv4 = 512,
        FSW = 1024,
        RAILWORKS = 2048,
        P3Dv5 = 4096,
        MSFS = 8192,
        FGFS = 16384, // FlightGear
        CUSTOM = 0x1000000,
    }

    public enum SimulationGamestate
    {
        Menu,
        Loading,
        Briefing,
        Flying
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
        PLAYSOUND,
        VIRTUALJOYSTICK,
        TEXT2SPEECH,
        SEPERATOR,
        PLATEIMAGE,
        PLATELABEL,
        SWITCHWINDOW,
        EXTERNAL,
        RESTCALL
    }
    public enum DataProviderMapType
    {
        Data,
        Control,
        Toggle,
        Exec
    }
    public enum EventPriority
    {
        First = 0,
        High = 1000,
        Low = 10000,
        Last = 100000,
        All = 999999,
    }

    public enum EventSeverity
    {
        None = 0,
        Verbose = 1,
        Normal = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    public enum VirtualJoystickAction
    {
        VJOY_BUTTON,
        VJOY_AXIS,
        VJOY_DISCRETE_HAT,
        VJOY_CONTINUOUS_HAT,
    }

    public enum VirtualJoystickButtonMode
    {
        NONE,
        PRESS,
        TRIGGER,
        RELEASE,
        PRESSANDRELEASE,
        RESPONSECURVE
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

    public enum XPLANE_ROLE : uint  // 1 for master, 2 for extern visual, 3 for IOS
    {
        UNKOWN,
        MASTER,
        VISUAL,
        IOS
    }

    public enum SPADValueOperation
    {
        Set,
        Increment,
        Decrement,
        SetBit,
        ClearBit,
        ToggleBit,
        AppendChars,
        DeleteChars,
        ClearChars,
        SetEventValue,
        SetEventValue_Inverted,
        SetEventValue_Normalized,
        SetEventValue_Normalized_Inverted,
        SetEventValue_AxisValue,
        SetEventValue_AxisPercent
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

    public enum SPADConditionType
    {
        CONDITION,
        EVENT
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

    public enum FLASHMODE : byte
    {
        FLASHMODE_STATIC = 0,
        FLASHMODE_SLOW = 1, // 2s
        FLASHMODE_FAST = 2, // 1s 
        FLASHMODE_FASTER = 3, // 0.5s
        FLASHMODE_ONCE = 4, // Flash LED once if it is on.
    }

    public enum SPADKeyboardOption
    {
        PRESSANDRELEASE = 0,
        PRESS = 1,
        RELEASE = 2,
        // Macro = 3
    }


    public enum SPADSwitchTypes
    {
        UNKNOWN = 0x0,
        PUSHBUTTON = 0x10000001,
        PUSHBUTTON_NOMODE = 0x00000001,
        SWITCH = 0x00000002,
        ENCODER = 0x00000004,
        LED = 0x00000008,
        DISPLAY = 0x00000010,
        VALUESWITCH = 0x00000020,
        LED_3COL = 0x00000040,
        ZZ_HAS_MODE = 0x10000000
    }

    [Flags]
    public enum PROGRAMMABLEBUTTONSTATUS
    {
        OFF = 0x0,
        SHORT = 0x1,
        LONG = 0x2,
        BOTH = 0x3,
    }

    public enum SPADButtonLight
    {
        OFF,
        BUTTON_SHORT_ON,
        BUTTON_SHORT_OFF,
        BUTTON_LONG_ON,
        BUTTON_LONG_OFF,
        ON
    }

    public enum JoystickAxis
    {
        AxisX, AxisY, AxisZ, AxisRX, AxisRY, AxisRZ
    }

    public enum DeviceInputTypes
    {
        Button,
        Axis,
        HatSwitch,
        VirtualHatSwitch,
        ModeSwitch,
        Switch,
        Led,
        Display,
        Encoder,
        Unkown
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
        NoRemotePasteThis = 64,
        NoRemotePasteAll = 128,
        NoRemotePasteDevice = 256,
        PanelClipboardAll = PanelCopyAll | PanelCopyThis | PanelPasteAll | PanelPasteThis | PanelCopyDevice | PanelPasteDevice,
    }

    public enum GaugePowerMode
    {
        Default,
        AlwaysOn,
        NoData
    }

    public enum GaugeCompatibility
    {
        Saitek,
        ESP,
        SPAD
    }

    public enum VARIABLE_SCOPE
    {
        SESSION,
        PROFILE,
        GAUGE,
        DEVICE,
        EVENT,
        PRIVATE,
        DONOTUSE,
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
        DIRECTORY_CACHE,
        DIRECTORY_APPDATAFIXED,
        // needed? DIRECTORY_DOCUMENTSFIXED,
    }

    public enum PANEL_BUTTONPOSITION
    {
        FIRST = 0,
        LAST = -1,
        DROPDOWN = 1,
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
        Repeat,/*
        Repeat50,
        Repeat100,
        Repeat200,
        Repeat400,
        Repeat500,
        Repeat1000,*/

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

    

    public enum REMOTE_SOCKET_COMMAND
    {
        
        PushImage = 1,
        PushImageNonFlipped,
        PushImageRaw,
        PushImageRawNonFlipped,
        LedChange,
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
        REMOTE

    }

    public enum Keys : int
    {
        //
        // Summary:
        //     No key pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        Clear = 12,
        //
        // Summary:
        //     The RETURN key.
        Return = 13,
        //
        // Summary:
        //     The ENTER key.
        Enter = 0x010d,
        //
        // Summary:
        //     The SHIFT key.
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        Menu = 18,
        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,
        //
        // Summary:
        //     The CAPS LOCK key.
        Capital = 20,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        HanguelMode = 21,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //
        // Summary:
        //     The ESC key.
        Escape = 27,
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
        IMEAceept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,
        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        Prior = 33,
        //
        // Summary:
        //     The PAGE UP key.
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        Next = 34,
        //
        // Summary:
        //     The PAGE DOWN key.
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        Snapshot = 44,
        //
        // Summary:
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        Insert = 45,
        //
        // Summary:
        //     The DEL key.
        Delete = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,
        //
        // Summary:
        //     The NUM LOCK key.
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        RMenu = 165,
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //
        // Summary:
        //     The OEM 1 key.
        Oem1 = 186,
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        Oemplus = 187,
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        Oemcomma = 188,
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //
        // Summary:
        //     The OEM 2 key.
        Oem2 = 191,
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        Oemtilde = 192,
        //
        // Summary:
        //     The OEM 3 key.
        Oem3 = 192,
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //
        // Summary:
        //     The OEM 4 key.
        Oem4 = 219,
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //
        // Summary:
        //     The OEM 5 key.
        Oem5 = 220,
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //
        // Summary:
        //     The OEM 6 key.
        Oem6 = 221,
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //
        // Summary:
        //     The OEM 7 key.
        Oem7 = 222,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = 223,
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //
        // Summary:
        //     The OEM 102 key.
        Oem102 = 226,
        //
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        Attn = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,

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
        public const string PlateImage = "__PLATEIMAGE__";
        public const string PlateLabel = "__PLATELABEL__";
        public const string Digitmark = "__DIGITMARK__";
        public const string Display = "DISPLAY";
        public const string LeftDisplay = "LEFTDISPLAY";
        public const string RightDisplay = "RIGHTDISPLAY";
        public const string UpdateLabel = "__UPDATELABEL__";
        public const string UpdateVariable = "__UPDATEVAR__";
        public const string ActiveWindow = "__ACTIVEWINDOW__";
        public const string FSXMainWindow = "FS98MAIN";
        public const string XPlaneMainWindow = "__XPLANEWINDOW__";
        public const string ModeChanged = "__MODECHANGED__";
        public const string ModeReport = "__MODEREPORT__";
        public const string EventUpdate = "__EVENTUPDATE__";
        public const string SystemEvent = "__SYSTEM__";
    }

    public static class SPADSystemEvents
    {
        public const string StatusMessage = "SPAD.StatusMessage";
        public const string AircraftChanged = "SPAD_Aircraft";
        public const string ProfileChanged = "SPAD_Profile";
        public const string FSUIPCStatus = "FSUIPC.Status";
        public const string SimConnectStatus = "SimConnect.Status";
        public const string ProviderStatus = "Provider.Status";
        public const string CDUAvailable = "SPAD_CDUAvailable";
        public const string CDUUpdate = "SPAD_CDU_Update_";
        public const string RequestAttention = "SPAD.RequestAttention";
        public const string ExtractXplane = "SPAD.ExtractXplane";
        public const string ProfileLoaded = "SPAD.ProfileLoaded";
        public const string ProgrammingModeChanged = "SPAD.ProgrammingModeChanged";
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
        public const string FRAMERATE = "FRAMERATE";
        public const string FRAMERATE_AVG = "FRAMERATE AVG";

        public const string TRANSPONDER1_DIGIT_1 = "TC1DIGIT1";
        public const string TRANSPONDER1_DIGIT_2 = "TC1DIGIT2";
        public const string TRANSPONDER1_DIGIT_3 = "TC1DIGIT3";
        public const string TRANSPONDER1_DIGIT_4 = "TC1DIGIT4";

        public const string TRANSPONDER2_DIGIT_1 = "TC2DIGIT1";
        public const string TRANSPONDER2_DIGIT_2 = "TC2DIGIT2";
        public const string TRANSPONDER2_DIGIT_3 = "TC2DIGIT3";
        public const string TRANSPONDER2_DIGIT_4 = "TC2DIGIT4";

        public const string CDU_0_AVAILABLE = "CDU_0_AVAILABLE";
        public const string CDU_1_AVAILABLE = "CDU_1_AVAILABLE";
        public const string CDU_2_AVAILABLE = "CDU_2_AVAILABLE";
        public const string CDU_0_PREFIX = "CDU_0_";
        public const string CDU_1_PREFIX = "CDU_1_";
        public const string CDU_2_PREFIX = "CDU_2_";
        public const string EVENT_AIRCRAFTCHANGED = "SPAD_AIRCRAFTCHANGED";
        public const string EVENT_AIRCRAFT = "SPAD_AIRCRAFT";
        public const string EVENT_PROFILE = "SPAD_PROFILE";
        public const string EVENT_EVENTVALUE = "SPAD_EVENTVALUE";
        public const string EVENT_EVENTVALUERAW = "SPAD_EVENTVALUERAW";
    }
    public class Constants
    {
        public const string Namespace = "http://www.fsgs.com/SPAD";
        public const string ConfigNamespace = "http://www.fsgs.com/SPAD/v2";
        public const string DataNamespace = "http://schemas.spadnext.com/";

        public const UInt32 DEFINE_ID_NOTREGISTERED = UInt32.MaxValue;

        public static readonly Guid EVENTACTION_NEWID = Guid.Empty;
        public static readonly Guid EVENTACTION_POWERON_GUID = new Guid("{60F846D4-79F9-48F9-9C7E-59CDFFA5AD9A}");
        public static readonly Guid EVENTACTION_POWEROFF_GUID = new Guid("{BF175192-C786-4526-873F-2362388A9445}");
        public static readonly Guid EVENTACTION_DISPLAY_GUID = new Guid("{F3BD2794-9EB4-4016-ABE2-052989F775A9}");

        public static readonly Guid DEVICE_NOTREGISTERED = new Guid("{A6C592BD-69A8-4F8B-8541-FBF9FB717A7E}");

        public const string FEATURE_GAUGEDESIGNER = "GaugeDesigner";

        public static readonly Guid Controller_FSUIPC = new Guid("{31F5957E-5A22-4470-B133-B457C8893323}");
        public static readonly Guid Controller_SIMCONNECT = new Guid("{74E84133-7FAD-413A-B06B-C300947905D5}");

        public const int FIP_MAX_VARIABLES = 1024;
        public static readonly List<SPADValueOperation> EventValueOperations = new List<SPADValueOperation>()
        {
            SPADValueOperation.SetEventValue,
            SPADValueOperation.SetEventValue_Normalized,
            SPADValueOperation.SetEventValue_Inverted,
            SPADValueOperation.SetEventValue_Normalized_Inverted,
            SPADValueOperation.SetEventValue_AxisValue,
            SPADValueOperation.SetEventValue_AxisPercent
        };
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

    public class DataConfiguration
    {
        public const string Data = "data";
        public const string SPAD = "spad";
        public const string Devices = "devices";
        public const string Gauges = "gauges";
        public const string Localization = "lang";
        public const string Special = "special";
        public const string Vendor = "vendor";
        public const string XPlane = "xplane";
    }
}
