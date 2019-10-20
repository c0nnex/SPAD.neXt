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
        bool IsOff { get; set; }
        bool LEDIsOn { get; }
        bool LongModeOn { get; set; }
        Visibility LongModeVisible { get; set; }
        ulong Mask { get; set; }
        bool ShortModeOn { get; set; }
        Visibility ShortModeVisible { get; set; }
        string Tag { get; }
        bool NeedsCallout { get; }

        bool HasMode(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeOff(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeOn(PROGRAMMABLEBUTTONSTATUS whichMode);
        void ModeToggle(PROGRAMMABLEBUTTONSTATUS whichMode);
        void Reset();
        void SetMode(PROGRAMMABLEBUTTONSTATUS newMode);
        CancellationTokenSource StartWaiting();
        void StopWaiting();
        bool UpdateSequence();
    }
}
