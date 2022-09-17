using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.HID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SPAD.neXt.Interfaces
{
    public interface IRemovableDevice
    {
        event EventHandler DeviceAttached;
        event EventHandler DeviceRemoved;
    }

    public interface IPanelDevice : IRemovableDevice
    {
        event EventHandler<IPanelDeviceEventArgs> DeviceReportReceived;

        Guid DeviceSessionId { get; }
        IDeviceConfiguration DeviceConfiguration { get; }
        IHidDeviceCapabilities Capabilities { get; }
        IHidDeviceAttributes Attributes { get; }
        IUSBDevice UsbDevice { get; }
        string DevicePath { get; }
        string ProductID { get; }
        string VendorID { get; }
        string DeviceTypeID { get; }
        string SerialNumber { get; }
        string Name { get; set; }
        string BaseName { get; set; }
        string DeviceID { get; }
        int DeviceIndexForEvents { get; set; }
        bool ProcessDeviceData { get; set; }
        bool IsConnected { get; }
        bool IsListening { get; }

        void SetDeviceConfiguration(IDeviceConfiguration deviceConfiguration);


        bool OpenDevice();
        void OpenHidDevice();
        void CloseDevice();

        void StartProcessing();
        void StopProcessing();
        void SendLastReport();
        void InjectReport(IPanelDeviceReport rep);

        bool Write(byte[] data);
        bool WriteFeatureData(byte[] data);
        bool ReadFeatureData(out byte[] data, byte reportId = 0);
        
    }

   

    public delegate void InputEventhandler(IInputDevice sender, AxisInputEventArgs e);
    public delegate void AxisEventHandler(IInput sender, AxisEventValue e);


    public sealed class AxisInputEventArgs
    {
        private static long eventNumber;

        public uint CustomIndex { get; }
        public IInput Input { get; private set; }
        public float Value { get; }
        public bool IsTriggered { get; }
        public string Name { get; }
        public string CustomName { get; }
        public int RawValue { get; }
        public bool IsStatusUpdate { get; }
        public string SwitchName { get; }
        public long EventNumber { get; }
        public ulong EventTick { get; }

        public AxisInputEventArgs()
        {
            EventNumber = Interlocked.Increment(ref eventNumber);
            EventTick = EnvironmentEx.TickCount;
        }

        public AxisInputEventArgs(IInput input) : this()
        {
            Input = input;
            CustomIndex = input.CustomIndex;
            Value = input.Value;
            IsTriggered = input.IsTriggered;
            Name = input.Name;
            RawValue = input.RawValue;
            IsStatusUpdate = false;
            CustomName = input.CustomName;
            SwitchName = Name.ToUpperInvariant().Replace(' ', '_');
            EventNumber = Interlocked.Increment(ref eventNumber);
            EventTick = input.LastEventTick;
        }

        public AxisInputEventArgs(IInput input, bool isStausUpdate) : this(input)
        {
            IsStatusUpdate = isStausUpdate;
        }

        public AxisInputEventArgs(string switchName,int rawValue,float value, bool isStatusUpdate = false) : this()
        {
            SwitchName = switchName;    
            RawValue=rawValue;
            Value = value;  
            IsStatusUpdate = isStatusUpdate;    
        }

        public AxisInputEventArgs WithInput(IInput input)
        {
            Input = input;
            return this;
        }
        public override string ToString()
        {
            return $"InputEventArgs Name={Name} CustomName={CustomName} CustomIndex={CustomIndex} Triggered={IsTriggered} SwitchName={SwitchName} Index={Input?.Index}";
        }
    }

    public interface ICalibrateableDevice
    {
        string DeviceCalibrationName { get; }
        IEnumerable<string> DeviceCalibrationNameAlternates { get; }
        string VendorID { get; }
        string ProductID { get; }
    }

    public interface IInputDevice 
    {
        string Name { get; }
        int Identifier { get; }
        IEnumerable<IInput> Inputs { get; }
        IEnumerable<IInput> AllInputs { get; }
        event InputEventhandler AxisChanged;
        event InputEventhandler ButtonDown;
        event InputEventhandler ButtonUp;
        event EventHandler<IInputDevice, ulong> InputUpdateCompleted;
        bool IsGamePad { get; set; }
        uint RegisterCustomIndex(string targetName, uint customIndex, bool hasPriority);
        uint RegisterCustomIndex(string targetName, Enum customIndex, bool hasPriority);
        uint RegisterCustomIndex(uint index, uint customIndex, bool hasPriority);
        uint RegisterCustomIndex(uint index, Enum customIndex, bool hasPriority);
        void RegisterCustomName(uint index, string customName);
        void RegisterCustomName(string targetName, string customName);
        void RegisterControls(IEnumerable<string> names, JoystickInputTypes inputType, int min = 0, int max = 0);
        void RegisterControl(string name, uint index, JoystickInputTypes inputType, int min = 0, int max = 0);
        void CreateVirtualHatSwitch(uint hatNumber, uint bNorth, uint bEast, uint bSouth, uint bWest,uint inputID);
        void SetPriority(string targetName);
        void SetMinimumChange(string targetName, int minChange);
        bool IsTriggered(string targetName);
        void SendCurrentStatus();

        void EnableInputEvents(string targetName);
        void DisableInputEvents(string targetName);
    }

    public interface IInput
    {
        event InputEventhandler ButtonDown;
        event InputEventhandler ButtonUp;
        event InputEventhandler AxisChanged;

        uint ID { get; }
        uint Index { get; }
        
        int DeadZone { get; }
        IInputDevice Device { get; }
        uint CustomIndex { get; }
        JoystickInputTypes InputType { get; }
        bool Invert { get; }
        bool IsTriggered { get; }
        bool HasPriority { get; set; }
        InputModifier Modifier { get; set; }
        string Name { get; }
        string CustomName { get; set; }
        string DisplayName { get; set; }
        float UnitRangeValue { get; }
        float Value { get; }
        float InvertedValue { get; }
        float PreviousValue { get; }
        int RawValue { get; }
        bool InputEventsEnabled { get; }
        bool IsEnabled { get; set; }
        ulong LastEventTick { get; }
        bool IsVirtual { get; }
        int ButtonGroup { get; }
        int ButtonGroupIndex { get; }

        float Normalize(int value, int minimum, int maximum);
        float Rescale(float value, float sourceMin, float sourceMax, float targetMin, float targetMax);
        float Sigmoid(float x, int curve);
        void Update(float value, int rawValue = 0);
        void SetCustomIndex(uint index);

        void EnableInputEvents();
        void DisableInputEvents();

        event AxisEventHandler InputChanged;
    }

    public interface IInputAxis : IInput
    {
        float[] Curve { get; set; }
        
        void SetCalibrationMode(bool bEnable);

        bool AxisNeedsUpdate { get; set; }
        int MinimumValue { get; set; }
        int MaximumValue { get; set; }
        int DeviceDeadzone { get; set; }
        int AxisSmoothValue { get; set; }
        int AxisMinimumDelta { get; set; }
        float NormalizedValue { get; }
        int AxisValue { get; }
        bool AntiJitterActivated { get; set; }

        IAxisCalibration DefaultCalibration { get; }
    }

    public interface IInputAxisConfiguration 
    {
        IInputRange LogicalRange { get;  }
        IInputRange PhysicalRange { get;  }
        IInputRange Range { get; set; }
    }

    public interface IInputRange
    {
        float Maximum { get; }
        float Minimum { get; } 
    }

    public interface IInputButton : IInput
    {
        IInputAxis VirtualAxis { get; }
    }

    public interface IInputHatswitchButton : IInputButton
    {
        JoystickHatDirection HatDirection { get; }
    }

    public interface IGameDevice : IInputDevice, IPanelDevice, ICalibrateableDevice
    {
        IInputDevice InputDevice { get; }
        IPanelDevice PanelDevice { get; }

        IControllableDevice GetControllableDevice();
    }

    public interface IControllableDevice
    {
        void SetBrightness(int index, int howBright);
        void SetLEDColor(int index, SPADColors color);
        void SetString(int index, string value);
        void ClearString(int index);
    }
}
