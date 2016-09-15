using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{
    public interface IEventActionRangedAxis : IEventAction
    {
        bool InvertAxis { get; set; }
        bool SendRaw { get; set; }
        bool Option1Selected { get; set; }
        bool Option2Selected { get; set; }
        bool Option3Selected { get; set; }
        bool Option4Selected { get; set; }
    }

    public interface IEventActionCustomAxis : IEventActionChangeValue
    {
        bool AlwaysActive { get; set; }
        SPADEventValueComparator Comparator { get; }
        bool InvertAxis { get; set; }
        bool OneTime { get; set; }
        int RangeFrom { get; set; }
        int RangeTo { get; set; }
        int MapFrom { get; set; }
        int MapTo { get; set; }
        bool UseMapping { get; set; }
        bool SendRaw { get; set; }
    }

    public interface IEventActionMonitor
    {
        IDataDefinition TargetDataDefinition { get; set; }
        IDataDefinition SourceDataDefinition { get; set; }
        string TargetDataDefinitionID { get; set; }
        string SourceDataDefinitionID { get; set; }
        IReadOnlyList<IDataDefinition> MonitoredDataDefinitions { get; }
    }

    public interface IEventActionChangeValue : IEventAction,IEventActionMonitor
    {
        double Value { get; set; }
        SPADValueOperation ValueOperation { get; set; }

        bool UseTriggerValue { get; set; }
        bool HasMinMax { get; set; }
        Double ValueMin { get; set; }
        Double ValueMax { get; set; }
        bool EnableRollOver { get; set; }
    }

    public interface IEventActionObserve : IEventAction
    {
        IDataDefinition TargetDataDefinition { get; set; }
        string TargetDataDefinitionID { get; set; }
        IReadOnlyList<IDataDefinition> ObservedDataDefinitions { get; }
    }

    public interface IEventActionSingleton : IEventAction
    {
        Guid SingletonID { get; }
    }

    public interface IEventActionCommand : IEventAction
    {
        string CommandParameter { get; set; }
        string CommandName { get; set; }
    }

    public interface IEventActionPlaySound : IEventAction
    {
        SPADSoundOperation SoundOperation { get; set; }
        int Volume { get; set; }
        string SoundName { get; set; }
    }

    public interface IEventActionDisplayValue :  IEventActionObserve
    {
        string TargetDisplay { get; set; }
        string DisplayFormat { get; set; }
    }

    public interface IEventActionKeyboard : IEventAction
    {
        IKeyMacro KeyboardMacro { get;}
        SPADKeyboardOption MacroType { get; set; }
        void ConfigureKeyboardAction(IEnumerable<int> keys, int duration, int pause, int repeat);
    }

    public interface IKeyMacro
    {
        IEnumerable<int> Keys { get; }
        string KeySequence { get; }
        bool IsPause { get; }
        int Duration { get; }
        int Pause { get; }
        int Repeat { get;}
    }

    public interface IEventActionJoystick : IEventAction
    {
        int Button { get; set; }
        int Joystick { get; set; }
    }

    public interface IEventActionDelay : IEventAction
    {
        uint Delay { get; set; }
    }

    public interface IEventActionScript : IEventAction
    {
        string ScriptName { get; set; }
        int ScriptArgument { get; set; }
    }
}
