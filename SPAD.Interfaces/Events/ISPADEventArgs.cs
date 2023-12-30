using SPAD.neXt.Interfaces.Extension;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace SPAD.neXt.Interfaces.Events
{
    public interface ISPADEventArgs : IHandledEventArgs
    {
        ConcurrentDictionary<string, object> EventData { get; }
        string EventSwitch { get; set; }
        string EventName { get; set; }
        string EventTrigger { get; set; }
        string NewValueFormatted { get; }
        string DisplayFormat { get; }
        object NewValue { get; }
        object OldValue { get; }
        string TargetDevice { get; }
        long EventMarker { get; }
        long Timestamp { get; }
        EventPriority EventPriority { get; }
        EventSeverity EventSeverity { get; }


        IDeviceProfile DeviceProfile { get; set; }
        IMonitorableValue MonitorableValue { get; set; }
        Guid Sender { get; }
        Guid ExecutionContext { get; }
        bool Immediate { get; set; }
        bool IsValueEvent { get; set; }
        bool IsCascadedEvent { get; set; }
        bool NoValueSet { get; set; }
        bool IsAxisEvent { get; set; }
        bool IsDisplayEvent { get; set; }
        bool IsThrottled { get; }
        string ThrottleID { get; }
        bool IsStateEvent { get; set; }
        string FullName { get; }
        string AdditionalInfo { get; set; }
        UInt64 CreationTime { get; }
        EventOperations EventOperation { get; set; }
        IInputElement CommandTarget { get; set; }
        void UpdateEventName(string eventName);
        void UpdateSender(Guid newSender);
        void SetHandled(string eventName);
        bool GetHandled(string eventName);
        void SetCallbackValue(object value);
        void Callback(IValueProvider provider);
        ISPADEventArgs AsThrottled();
        ISPADEventArgs Clone();
        T GetData<T>(string key, T defaultValue = default(T));
        ISPADEventArgs WithData(string key, object data);
        ISPADEventArgs WithDataIfNot<T>(string key, T data, T compareValue) where T: IEquatable<T>;
        bool Is(ISPADEventArgs e);

        ISPADEventArgs WithEventParameter(string name, object value);
        object GetEventParameter(string name);
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

        public ConcurrentDictionary<string, object> EventData { get; private set; } = new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public static new SPADEventArgs Empty = new SPADEventArgs();
        private static long EventMarkerCounter = 0;
        public string EventSwitch { get; set; }
        public string EventName { get => eventName; set { eventName = value; _FullName = null; } }
        public string EventTrigger { get => eventTrigger; set { eventTrigger = value; _FullName = null; } }
        public IInputElement CommandTarget { get; set; }
        public long EventMarker { get; private set; }
        public long Timestamp { get; set; }
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
        public bool NoValueSet { set; get; } = false;
        public bool IsCascadedEvent { get; set; }
        public bool IsAxisEvent { get; set; }
        public object CallbackValue { get; set; }
        public bool IsDisplayEvent { get; set; }
        public bool IsStateEvent { get; set; }
        public bool IsThrottled { get; private set; }
        public string ThrottleID { get; private set; }
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
            EventMarker = Interlocked.Increment(ref EventMarkerCounter);
            Timestamp = EnvironmentEx.TickCountLong;
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

        public SPADEventArgs(string boundTo, string trigger) : this(boundTo)
        {
            EventTrigger = trigger;
        }

        public SPADEventArgs(string eventName, object newValue, object oldValue)
            : this(eventName)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            NewValueFormatted = newValue?.ToString();
        }

        public SPADEventArgs(string eventName, string eventTrigger, object newValue, object oldValue)
         : this(eventName, eventTrigger)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            NewValueFormatted = newValue?.ToString();
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
        public void UpdateSender(Guid newSender) {  Sender = newSender; }
        public ISPADEventArgs AsThrottled()
        {
            ThrottleID = FullName + GetData("LAYER", "");
            IsThrottled = true;
            return this;
        }

        public override string ToString()
        {
            var s = $"{FullName} {EventMarker} Old={OldValue} New={NewValue} Sw={EventSwitch} VE={IsValueEvent} {Immediate} {ExecutionContext}";
            if (EventParameters != null && EventParameters.Count > 0)
            {
                s += " [";
                foreach (var item in EventParameters)
                {
                    s += $"{item.Key}={item.Value},";
                }
                s += "]";
            }
            return s;
        }
        private string _FullName = null;
        public string FullName
        {
            get
            {
                if (_FullName == null)
                {
                    if (!String.IsNullOrEmpty(EventTrigger))
                        _FullName = String.Format("{0}.{1}", EventName, EventTrigger);
                    else
                        _FullName = EventName;
                }
                return _FullName;
            }
        }

        public object this[string key]
        {
            get
            {
                object val;
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
            this[key] = value;
        }

        public SPADEventArgs WithData(string key, object value)
        {
            AddData(key, value);
            return this;
        }

        ISPADEventArgs ISPADEventArgs.WithData(string key, object value)
        {
            AddData(key, value);
            return this;
        }
        ISPADEventArgs ISPADEventArgs.WithDataIfNot<T>(string key, T value, T compareValue) 
        {
            if (!compareValue.Equals(value))
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
                object val = this[key];
                if (val == null)
                    return defaultValue;
                if (val is T)
                    return (T)val;
                if (typeof(T) == typeof(Guid))
                    return (T)((object)new Guid(Convert.ToString(val, CultureInfo.InvariantCulture)));
                if (typeof(T) == typeof(Version))
                    return (T)((object)new Version(Convert.ToString(val, CultureInfo.InvariantCulture)));
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), Convert.ToString(val, CultureInfo.InvariantCulture));
                return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                logger?.Warn($"GetData " + typeof(T) + " => " + ex.Message);
                return defaultValue;
            }

        }

        private HashSet<string> handledEvents = new HashSet<string>();
        private string eventName;
        private string eventTrigger;

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

        public bool Is(ISPADEventArgs e) => FullName == e.EventName;

        private ConcurrentDictionary<string,object> EventParameters;
        public ISPADEventArgs WithEventParameter(string name, object value)
        {
            if (EventParameters == null) { EventParameters = new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase); }
            if (value == null) { EventParameters.TryRemove(name, out var _); return this; }
            EventParameters[name] = value;
            return this;
        }
        public object GetEventParameter(string name)
        {
            if (EventParameters == null) { return null; }
            if (EventParameters.TryGetValue(name, out var val))
                return val;
            return null;
        }
    }
}
