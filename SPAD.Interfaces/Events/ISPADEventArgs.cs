using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace SPAD.neXt.Interfaces.Events
{
    public interface ISPADEventArgs : IHandledEventArgs
    {
        string EventSwitch { get; set; }
        string EventName { get; set; }
        string EventTrigger { get; set; }
        string NewValueFormatted { get; }
        object NewValue { get;  }        
        object OldValue { get; }       
        string   TargetDevice { get; }
        IDeviceProfile DeviceProfile { get; set; }
        IMonitorableValue MonitorableValue { get; set; }
        Guid Sender { get; }

        bool Immediate { get; set; }
        bool IsValueEvent { get; set; }
        bool IsCascadedEvent { get; set; }
        string FullName { get; }
        string AdditionalInfo { get; set; }
        UInt64 CreationTime { get; }
        EventOperations EventOperation { get; set; }
        IInputElement CommandTarget { get; set; }
        void UpdateEventName(string eventName);
        void SetHandled(string eventName);
        bool GetHandled(string eventName);
        void SetCallbackValue(object value);
        void Callback(IValueProvider provider);

        ISPADEventArgs Clone();
    }
    
    public interface IHandledEventArgs
    {
        bool Handled { get; set; }
    }


    public class SPADEventArgs : HandledEventArgs, ISPADEventArgs
    {
        public static SPADEventArgs Empty = new SPADEventArgs();

        public string EventSwitch { get; set; }
        public string EventName { get;  set; }
        public IInputElement CommandTarget { get; set; }
        public virtual object OldValue { 
            get {
                if (MonitorableValue == null)
                    return oldValue;
                else
                    return MonitorableValue.PreviousValue;
            }
        }

        public SPADEventArgs WithTarget(string deviceTargetID)
        {
            TargetDevice = deviceTargetID;
            return this;
        }

        private object oldValue = null;
        public virtual object NewValue 
        {
            get
            {
                if (MonitorableValue == null)
                    return newValue;
                else
                    return MonitorableValue.CurrentValue;
            }
        } private object newValue = null;
        public string NewValueFormatted { get; set; }

        public virtual IMonitorableValue MonitorableValue { get; set; }
        public IDeviceProfile DeviceProfile { get; set; }
        public bool Immediate { get; set; }
        public bool IsValueEvent { get; set; }
        public string EventTrigger { get; set; }
        public bool IsCascadedEvent { get; set; }
       
        public object CallbackValue { get; set; }
        public string AdditionalInfo { get; set; }
        public string TargetDevice { get; set; }
        public UInt64 CreationTime { get; } = EnvironmentEx.TickCount64;
        public EventOperations EventOperation { get; set; } = EventOperations.Normal;
        public Guid Sender { get; set; } = Guid.Empty;

        private SPADEventArgs() { }

        public SPADEventArgs(string eventName)
        {
            EventName = eventName;
            EventTrigger = String.Empty;
            string[] args = eventName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if ( args.Length == 2)
            {
                EventName = args[0];
                EventTrigger = args[1];
            }
            this.Handled = false;
            Immediate = false;
            IsValueEvent = false;
            CallbackValue = null;
        }

        public SPADEventArgs(string boundTo,string trigger)
        {
            EventName = boundTo;
            EventTrigger = trigger;
            this.Handled = false;
            Immediate = false;
            IsValueEvent = false;
            CallbackValue = null;
        }

        public SPADEventArgs(string eventName, object newValue, object oldValue)
            : this(eventName)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public SPADEventArgs(string eventName, string eventTrigger, object newValue, object oldValue)
         : this(eventName, eventTrigger)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        /*
        public SPADEventArgs(string eventName, object newValue, object oldValue, IDeviceProfile deviceProfile)
            : this(eventName, newValue, oldValue)
        {
            this.DeviceProfile = deviceProfile;
        }*/

        public virtual void UpdateEventName(string eventName)
        {
            EventName = eventName;
            EventTrigger = String.Empty;
            string[] args = eventName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if ( args.Length == 2)
            {
                EventName = args[0];
                EventTrigger = args[1];
            }
        }

        public override string ToString()
        {
            return String.Format("{0} Old={1} New={2} ValueEvent={3}", FullName, Convert.ToString(OldValue), Convert.ToString(NewValue),IsValueEvent);
        }

        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(EventTrigger))
                    return String.Format("{0}.{1}", EventName, EventTrigger);
                return EventName;
            }
        }

        private HashSet<string> handledEvents = new HashSet<string>();
        public void SetHandled(string eventName)
        {
            handledEvents.Add(eventName);
        }
        public bool GetHandled(string eventName)
        {
            return handledEvents.Contains(eventName);
        }

        public void Callback(IValueProvider provider)
        {
            if ((provider != null) && (CallbackValue != null))
            {
                provider.EventCallback(CallbackValue);
            }
        }




        public void SetCallbackValue(object value)
        {
            CallbackValue = value;
        }

        public ISPADEventArgs Clone()
        {
            return this.MemberwiseClone() as ISPADEventArgs;
        }
    }
}
