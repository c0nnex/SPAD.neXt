using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace SPAD.neXt.Interfaces.Events
{
    public interface ISPADEventArgs : IHandledEventArgs
    {
        ConcurrentDictionary<string, string> EventData { get;}
        string EventSwitch { get; set; }
        string EventName { get; set; }
        string EventTrigger { get; set; }
        string NewValueFormatted { get; }
        string DisplayFormat { get; }
        object NewValue { get; }
        object OldValue { get; }
        string TargetDevice { get; }
        ulong EventMarker { get; }
        EventPriority EventPriority { get; } 
        EventSeverity EventSeverity { get; } 
        string this[string key] { get; set; }

        IDeviceProfile DeviceProfile { get; set; }
        IMonitorableValue MonitorableValue { get; set; }
        Guid Sender { get; }
        Guid ExecutionContext { get; }
        bool Immediate { get; set; }
        bool IsValueEvent { get; set; }
        bool IsCascadedEvent { get; set; }
        bool IsAxisEvent { get; set; }
        bool IsDisplayEvent { get; set; }
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
        T GetData<T>(string key, T defaultValue = default(T));
    }

    public interface IHandledEventArgs
    {
        bool Handled { get; set; }
    }

    public interface IAcceleratedEncoder
    {
        
        void Reset();
        int GetAcceleration(double threshold, double timeout, double multiplier, double maxAcceleration);
        IAcceleratedEncoder WasPressed(long tick = 0);
    }

    public sealed class SPADEventArgs : HandledEventArgs, ISPADEventArgs
    {
        private ILogger logger = null;

        public ConcurrentDictionary<string, string> EventData { get; private set; } = new ConcurrentDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public static new SPADEventArgs Empty = new SPADEventArgs();
        private static ulong EventMarkerCounter = 0;
        public string EventSwitch { get; set; }
        public string EventName { get; set; }
        public IInputElement CommandTarget { get; set; }
        public ulong EventMarker { get; private set; }
        public object OldValue
        {
            get
            {
                if (IsValueEvent || (MonitorableValue == null))
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
        public SPADEventArgs WithAdditionalInfo(string info)
        {
            AdditionalInfo = info;
            return this;
        }

        private object oldValue = null;
        public object NewValue
        {
            get
            {
                if (IsValueEvent || (MonitorableValue == null))
                    return newValue;
                else
                    return MonitorableValue.CurrentValue;
            }
        }
        private object newValue = null;
        public string NewValueFormatted { get; set; }
        public string DisplayFormat { get; set; }
        public IMonitorableValue MonitorableValue { get; set; }
        public IDeviceProfile DeviceProfile { get; set; }
        public bool Immediate { get; set; }
        public bool IsValueEvent { get; set; }
        public string EventTrigger { get; set; }
        public bool IsCascadedEvent { get; set; }
        public bool IsAxisEvent { get; set; }
        public object CallbackValue { get; set; }
        public bool IsDisplayEvent { get; set; }


        public string AdditionalInfo { get; set; }
        public string TargetDevice { get; set; }
        public UInt64 CreationTime { get; } = EnvironmentEx.TickCount64;
        public EventOperations EventOperation { get; set; } = EventOperations.Normal;
        public Guid Sender { get; set; } = Guid.Empty;
        public Guid ExecutionContext { get; set; } = Guid.Empty;
        public EventPriority EventPriority { get; set; } = EventPriority.Low;
        public EventSeverity EventSeverity { get; set; } = EventSeverity.Verbose;
        private SPADEventArgs() { }
        private readonly ulong CreationTimeStamp = EnvironmentEx.TickCount64;

        public SPADEventArgs(string eventName)
        {
            EventMarker = EventMarkerCounter++;
            EventName = eventName;
            EventTrigger = String.Empty;
            string[] args = eventName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 2)
            {
                EventName = args[0];
                EventTrigger = args[1];
            }
            this.Handled = false;
            Immediate = false;
            IsValueEvent = false;
            CallbackValue = null;
        }

        public SPADEventArgs(string boundTo, string trigger)
        {
            EventMarker = EventMarkerCounter++;
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

        public void UpdateEventName(string eventName)
        {
            EventName = eventName;
            EventTrigger = String.Empty;
            string[] args = eventName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 2)
            {
                EventName = args[0];
                EventTrigger = args[1];
            }
        }

        public override string ToString()
        {
            return String.Format("{0} Old={1} New={2} Switch={3} ValueEvent={4} {5}", FullName, Convert.ToString(OldValue, CultureInfo.InvariantCulture), Convert.ToString(NewValue,CultureInfo.InvariantCulture),EventSwitch, IsValueEvent,  ExecutionContext == Guid.Empty ? "" : ExecutionContext.ToString());
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

        public string this[string key]
        {
            get
            {
                string val;
                if (EventData.TryGetValue(key, out val))
                    return val;
                return null;
            }
            set
            {
                EventData.Add(key, value);
            }
        }

        public void AddData(string key, object value)
        {
            this[key] = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public SPADEventArgs WithData(string key,object value)
        {
            AddData(key, value);
            return this;
        }

        public SPADEventArgs WithLogger(ILogger logger)
        {
            this.logger = logger;
            return this;
        }

        public T GetData<T>(string key, T defaultValue = default(T))
        {
            try
            {
                string val = this[key];
                if (val == null)
                    return default(T);
                if (typeof(T) == typeof(Guid))
                    return (T)((object)new Guid(this[key]));
                if (typeof(T) == typeof(Version))
                    return (T)((object)new Version(this[key]));
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), val);
                return (T) Convert.ChangeType(val, typeof(T));
            }
            catch 
            {
                return defaultValue;
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
            if (logger != null)
            {
                logger.Debug($"EventHandling done in {EnvironmentEx.TickCount64 - CreationTime} ms {this.ToString()}");
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

        public ISPADEventArgs WithData(Dictionary<string, string> eventData)
        {
            foreach (var item in eventData)
            {
                EventData.Add(item.Key, item.Value);
            }
            return this;
        }

        public ISPADEventArgs WithSeverity(EventSeverity severity)
        {
            EventSeverity = severity;
            return this;
        }

        public ISPADEventArgs WithPriority(EventPriority priority)
        {
            EventPriority = priority;
            return this;
        }
        public ISPADEventArgs WithContext(Guid context)
        {
            ExecutionContext = context;
            return this;
        }
        public ISPADEventArgs AsAxisEvent()
        {
            IsAxisEvent = true;
            return this;
        }
    }
}
