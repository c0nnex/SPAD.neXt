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
    public interface IProgrammableButton : INotifyPropertyChanged
    {
        event EventHandler<IProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeTurnedOn;
        event EventHandler<IProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeTurnedOff;
        event EventHandler<IProgrammableButton, PROGRAMMABLEBUTTONSTATUS> ModeChanged;
        event EventHandler<IProgrammableButton, bool> LongOrShortPress;


        long LastPressDuration { get; }
        bool BothModeOn { get; set; }
        Visibility BothModeVisible { get; set; }
        ulong CurrentMask { get; set; }
        PROGRAMMABLEBUTTONSTATUS CurrentMode { get; }
        PROGRAMMABLEBUTTONSTATUS DefaultMode { get; set; }
        Visibility CurrentUIState { get; set; }
        Visibility CurrentUIStateInverted { get; set; }
        object Data { get; }
        I GetData<I>() where I : class;
        I GetRoutedToData<I>() where I : class;
        I GetUIRoutedToData<I>() where I : class;

        bool IsOff { get; set; }
        bool LEDIsOn { get; }
        bool LongModeOn { get; set; }
        bool NoActivation { get; set; }
        bool NoImplicitActivation { get; set; }
        Visibility LongModeVisible { get; set; }
        ulong Mask { get; set; }
        bool ShortModeOn { get; set; }
        Visibility ShortModeVisible { get; set; }
        string Tag { get; }
        string TargetTag { get; }
        bool HasUITargetTag { get; }
        string UITargetTag { get; }
        bool UINoReleaseUpdate { get; }
        string Label { get; }
        bool RefreshContext { get; set; }
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
        void Reset();
        bool SetMode(PROGRAMMABLEBUTTONSTATUS newMode);
        CancellationTokenSource StartWaiting();
        void StopWaiting();
        bool UpdateSequence();

    }
}
