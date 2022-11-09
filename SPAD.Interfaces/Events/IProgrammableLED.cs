using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SPAD.neXt.Interfaces.Events
{

    public interface IProgrammableInputUIState : INotifyPropertyChanged
    {
        string Tag { get; set; }
        Visibility Visibility { get; set; }
        int StateValue { get; set; }
        int StateUIValue { get; set; }
        string StateName { get; set; }
        string EventName { get; set; }
        void SetVisible(bool isVisible);
    }

    public sealed class ProgrammableInputStateChangedArgs : EventArgs
    {
        public ProgrammableInputStateChangedArgs(string tag, int stateValue, string stateName = null, string eventName = null)
        {
            Tag = tag;
            StateValue = stateValue;
            StateName = stateName;
            EventName = eventName;
        }
        public bool IsImmuneToVirtualPower { get; set; } = false;
        public string Tag { get; set; }
        public int StateValue { get; set; }
        public string StateName { get; set; }
        public string EventName { get; set; }

        public int StateIndexToggle { get; set; } = 0;
        public ProgrammableInputStateChangedArgs AsIntermediate()
        {
            IsIntermediate = true;
            return this;
        }
        public bool IsIntermediate { get; set; } = false;
        

        public ProgrammableInputStateChangedArgs AsStateInit()
        {
            IsStateInit = true;
            return this;
        }
        public bool IsStateInit { get; set; } = false;

        public override string ToString()
        {
            return $"InputStateChanged {Tag} {StateValue} {StateName} {EventName} {IsIntermediate} {IsStateInit}";
        }
    }

    public interface IProgrammableHeld
    {
        void StartHeld(CancellationToken cancellationToken);
    }

    public interface IProgrammableInput : INotifyPropertyChanged,IDisposable,IObjectWithOptions
    {
        event EventHandler<IProgrammableInput,ProgrammableInputStateChangedArgs> InputStateChanged;
        IProgrammableInputUIState CurrentUIState { get; set; }

        Guid Owner { get; set; }
        bool NoActivation { get; set; }
        bool NoImplicitActivation { get; set; }
        string Tag { get; }
        string TargetTag { get; }
        bool HasUITargetTag { get; }
        string UITargetTag { get; }
        bool UINoReleaseUpdate { get; }
        string Label { get; }
        bool RefreshContext { get; set; }
        string VariableName { get; set; }
        bool EnablePressAcceleration { get; set; }
        bool IsImmuneToStateInit { get; set; }
        int CurrentStateValue { get; set; }
        I GetData<I>() where I : class;
        I GetRoutedToData<I>() where I : class;
        I GetUIRoutedToData<I>() where I : class;
        void Reset();

        INPUT_CHANGE_DIRECTION SetState(string newState, int newValue, bool raiseEvent);

        void EnableHeldMode(IProgrammableHeld callback);
        void DisableHeldMode();

    }

    public enum INPUT_CHANGE_DIRECTION
    {
        NONE,
        CLOCKWISE,
        COUNTERCLOCKWISE
    }

    public interface IProgrammableLabel
    {
        string Label { get; set; }
        string LabelDefault { get; set; }
        Guid LabelSource { get; set; }
    }

    public interface IProgrammableRotary :  IProgrammableStatefulInput
    {
       
    }

    public interface IProgrammableStatefulInput : IProgrammableInput
    {
        IProgrammableInputUIState RegisterPostion(string positionName,string eventName, int value, int uiValue);
        IProgrammableInputUIState GetUIStateByName(string positionName);
        IProgrammableInputUIState GetUIStateByValue(int positionValue);
        IProgrammableInputUIState GetUIStateByNameOrValue(string positionName,int positionValue);
        INPUT_CHANGE_DIRECTION SetState(IProgrammableInputUIState newState, bool raiseEvent);
        IEnumerable<string> GetStateNames();
        IEnumerable<int> GetStateValues();
        IEnumerable<IProgrammableInputUIState> GetStates();
    }



    public interface IProgrammableSwitch : IProgrammableInput
    {
        string EventSwitchOnName { get; set; }
        string EventSwitchOffName { get; set; }

        IProgrammableInputUIState UIState_SwitchON { get;  }
        IProgrammableInputUIState UIState_SwitchOFF { get; }

    }

    [Obsolete("Do not use anymore. Will be unsupported in 0.9.13+")]
    public interface IOldProgrammableButton : IProgrammableInput, INotifyPropertyChanged
    {
        event EventHandler<IOldProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeTurnedOn;
        event EventHandler<IOldProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeTurnedOff;
        event EventHandler<IOldProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeChanged;
        event EventHandler<IOldProgrammableButton, bool> LongOrShortPress;


        long LastPressDuration { get; }
        bool BothModeOn { get; set; }
        Visibility BothModeVisible { get; set; }
        ulong CurrentMask { get; set; }
        PROGRAMMABLEBUTTONSTATUS CurrentMode { get; }
        PROGRAMMABLEBUTTONSTATUS DefaultMode { get; set; }
        new Visibility CurrentUIState { get; set; }
        Visibility CurrentUIStateInverted { get; set; }
        object Data { get; }

        bool IsOff { get; set; }
        bool LEDIsOn { get; }
        bool LongModeOn { get; set; }
        Visibility LongModeVisible { get; set; }
        ulong Mask { get; set; }
        bool ShortModeOn { get; set; }
        Visibility ShortModeVisible { get; set; }
        string EventPressName { get; set; }
        string EventLongPressName { get; set; }
        string EventReleaseName { get; set; }
        string EventShortOnName { get; set; }
        string EventShortOffName { get; set; }
        string EventLongOnName { get; set; }
        string EventLongOffName { get; set; }
        string EventHeldName { get; set; }


        string EventSwitchOnName { get; set; }
        string EventSwitchOffName { get; set; }
        bool NeedsCallout { get; }

        void SetLongPressThreshold(int newThreshold);

        bool IsHeldModeEnabled { get; }
        bool IsSwitch { get; }
        void EnableHeldMode();
        void DisableHeldMode();

        bool HasMode(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeOff(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeOn(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeToggle(PROGRAMMABLEBUTTONSTATUS whichMode);
        bool SetMode(PROGRAMMABLEBUTTONSTATUS newMode);
        CancellationTokenSource StartWaiting();
        void StopWaiting();
        bool UpdateSequence();

    }


    public interface IProgrammableButton : IProgrammableInput
    {

        long LastPressDuration { get; }
        string EventPressName { get; set; }
        string EventShortPressName { get; set; }
        string EventLongPressName { get; set; }
        string EventReleaseName { get; set; }
        string EventHeldName { get; set; }
        bool IsLongShortEnabled { get; set; }
        void SetLongPressThreshold(int newThreshold);

 
        void UpdateUI();
    }
}
