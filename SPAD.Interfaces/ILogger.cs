using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces.Logging
{
    public interface ILogProvider
    {
        event EventHandler<string> LogEntryAdd;
        IEnumerable<string> LogEntries { get; }
    }

    public interface ILogFactory
    {
        ILogger GetLogger(string name);
        ILogger GetExtensionLogger(string name);
    }

    public interface ILogRule
    {
        string Pattern { get; set; }
        SPADLogLevel LogLevel { get; set; }
        bool IsFinal { get; set; }
        bool IsSyncRule { get; set; }
    }

    [Serializable]
    public sealed class LogRule : ILogRule
    {
        [XmlAttribute]
        public string Pattern { get; set; }
        [XmlAttribute]
        public SPADLogLevel LogLevel { get; set; } = SPADLogLevel.Debug;
        [XmlAttribute]
        public bool IsFinal { get; set; } = true;
        [XmlAttribute]
        public bool IsSyncRule { get; set; } = false;

        public LogRule() { }
        public LogRule(string pattern, SPADLogLevel logLevel = SPADLogLevel.Debug, bool isFinal = true, bool isSyncRule = false)
        {
            Pattern = pattern;
            LogLevel = logLevel;
            IsFinal = isFinal;
            IsSyncRule = isSyncRule;
        }
    }

    public interface ILogger
    {
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Debug level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Debug level, otherwise it returns
        //     false.
         bool IsDebugEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Error level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Error level, otherwise it returns
        //     false.
         bool IsErrorEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Fatal level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Fatal level, otherwise it returns
        //     false.
         bool IsFatalEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Info level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Info level, otherwise it returns
        //     false.
         bool IsInfoEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Trace level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Trace level, otherwise it returns
        //     false.
         bool IsTraceEnabled { get; }
        //
        // Summary:
        //     Gets a value indicating whether logging is enabled for the Warn level.
        //
        // Returns:
        //     A value of true if logging is enabled for the Warn level, otherwise it returns
        //     false.
         bool IsWarnEnabled { get; }

        string Name { get; }
        string LoggerName { get; set; }
        void StatusInfo(string msg);
        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);


        void WarnWithNotification(string message, params object[] args);
        void WarnWithNotificationIgnoreable(string tag, string message, params object[] args);
        void WarnWithNotificationIf(bool doNotify, Func<string> p);
        void WarnWithNotificationIf(bool doNotify, string message, params object[] args);
        void WarnWithNotificationIfOnce(bool doNotify,string tag, Func<string> p);
        void WarnWithNotificationIfOnce(bool doNotify, string tag, string message, params object[] args);
        void ErrorWithNotification(string message, params object[] args);


        void SetMinLogLevel(SPADLogLevel level);
        void Trace(Func<string> p);
        void Debug(Func<string> p);
        void Info(Func<string> p);
        void Warn(Func<string> p);

        event EventHandler<SPADLogLevel, string> OnLog;

        ILogger CreateChildLogger(string name);
    }


    public enum SPADLogLevel
    {
        None = 0,
        Error = 1,
        Warn = 2,
        Info = 3,
        Debug = 4,
        Trace = 5,
        All = 255,
    }

    public sealed class NullLogger : ILogger
    {
        public bool IsDebugEnabled => false;

        public bool IsErrorEnabled => false;

        public bool IsFatalEnabled => false;

        public bool IsInfoEnabled => false;

        public bool IsTraceEnabled => false;

        public bool IsWarnEnabled => false;

        public string Name { get; set; }

        public string LoggerName { get; set; }

        public event EventHandler<SPADLogLevel, string> OnLog;

        public ILogger CreateChildLogger(string name)
        {
            return new NullLogger();
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void Debug(Func<string> p)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void ErrorWithNotification(string message, params object[] args)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        public void Info(Func<string> p)
        {
        }

        public void SetMinLogLevel(SPADLogLevel level)
        {
        }

        public void Trace(string message, params object[] args)
        {
        }

        public void Trace(Func<string> p)
        {
        }

        public void Warn(string message, params object[] args)
        {
        }

        public void Warn(Func<string> p)
        {
        }
        public void StatusInfo(string msg) { }
        public void WarnWithNotification(string message, params object[] args)
        {
        }

        public void WarnWithNotificationIf(bool doNotify, Func<string> p)
        {
        }

        public void WarnWithNotificationIf(bool doNotify, string message, params object[] args)
        {
        }

        public void WarnWithNotificationIfOnce(bool doNotify, string tag, Func<string> p)
        {
        }

        public void WarnWithNotificationIfOnce(bool doNotify, string tag, string message, params object[] args)
        {
        }

        public void WarnWithNotificationIgnoreable(string tag, string message, params object[] args)
        {
         
        }
    }
    public sealed class ConsoleLogger : ILogger
    {
        private SPADLogLevel minLogLevel = SPADLogLevel.Debug;
        public bool IsDebugEnabled => minLogLevel >= SPADLogLevel.Debug;

        public bool IsErrorEnabled => minLogLevel >= SPADLogLevel.Error;

        public bool IsFatalEnabled => true;

        public bool IsInfoEnabled => minLogLevel >= SPADLogLevel.Info;

        public bool IsTraceEnabled => minLogLevel >= SPADLogLevel.Trace;

        public bool IsWarnEnabled => minLogLevel >= SPADLogLevel.Warn;

        public string Name { get; set; }

        public string LoggerName { get; set; }

        public event EventHandler<SPADLogLevel, string> OnLog;

        public ILogger CreateChildLogger(string name)
        {
            return new ConsoleLogger();
        }

        private void Log(SPADLogLevel level, string message, params object[] args) 
        {
            OnLog?.Invoke(level, message);
            Console.WriteLine(Name +"|" + level+"|"+ (args != null && args.Length>0? String.Format(message, args) : message));
        }

        public void Debug(string message, params object[] args)
        {
            if (!IsDebugEnabled) return;
            Log(SPADLogLevel.Debug, message, args);
        }

        public void Debug(Func<string> p)
        {
            if (!IsDebugEnabled) return;
            Log(SPADLogLevel.Debug, p());
        }

        public void Error(string message, params object[] args)
        {
            if (!IsErrorEnabled) return;
            Log(SPADLogLevel.Error, message, args);
        }

        public void ErrorWithNotification(string message, params object[] args)
        {
            if (!IsErrorEnabled) return;
            Log(SPADLogLevel.Error, message, args);
        }

        public void Info(string message, params object[] args)
        {
            if (!IsInfoEnabled) return;
            Log(SPADLogLevel.Info, message, args);
        }

        public void Info(Func<string> p)
        {
            if (!IsInfoEnabled) return;
            Log(SPADLogLevel.Info, p());
        }

        public void SetMinLogLevel(SPADLogLevel level)
        {
            minLogLevel = level;
        }

        public void Trace(string message, params object[] args)
        {
            if (!IsTraceEnabled) return;
            Log(SPADLogLevel.Trace, message, args);
        }

        public void Trace(Func<string> p)
        {
            if (!IsTraceEnabled) return;
            Log(SPADLogLevel.Trace, p());
        }

        public void Warn(string message, params object[] args)
        {
            if (!IsWarnEnabled) return;
            Log(SPADLogLevel.Warn, message, args);
        }

        public void Warn(Func<string> p)
        {
            if (!IsWarnEnabled) return;
            Log(SPADLogLevel.Warn, p());
        }
        public void StatusInfo(string msg) => Info(msg);
        public void WarnWithNotification(string message, params object[] args)
        {
            if (!IsWarnEnabled) return;
            Log(SPADLogLevel.Warn, message, args);
        }

        public void WarnWithNotificationIf(bool doNotify, Func<string> p)
        {
            if (!IsWarnEnabled || !doNotify) return;
            Log(SPADLogLevel.Warn, p());

        }

        public void WarnWithNotificationIf(bool doNotify, string message, params object[] args)
        {
            if (!IsWarnEnabled || !doNotify) return;
            Log(SPADLogLevel.Warn, message, args);

        }

        public void WarnWithNotificationIfOnce(bool doNotify, string tag, Func<string> p)
        {
            if (!IsWarnEnabled || !doNotify) return;
            Log(SPADLogLevel.Warn, p());

        }

        public void WarnWithNotificationIfOnce(bool doNotify, string tag, string message, params object[] args)
        {
            if (!IsWarnEnabled || !doNotify) return;
            Log(SPADLogLevel.Warn, message, args);

        }

        public void WarnWithNotificationIgnoreable(string tag, string message, params object[] args)
        {
            Log(SPADLogLevel.Warn, message, args);
        }
    }
}
