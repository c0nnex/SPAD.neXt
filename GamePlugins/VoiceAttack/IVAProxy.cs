using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{
    /// <summary>
    /// VoiceAttack V4 Interface
    /// </summary>
    public static class vaProxy
    {
        private static dynamic _vaProxy;

        public static void SetVAProxy(dynamic vaproxy)
        {
            _vaProxy = vaproxy;
        }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return (GetBoolean("snDEBUG")).GetValueOrDefault(true);
#else
                return (GetBoolean("snDEBUG")).GetValueOrDefault(false);
# endif
            }
        }
        public static bool IsVerbose
        {
            get
            {
                return (GetBoolean("snVERBOSE")).GetValueOrDefault(true);
            }
        }
        public static bool CommandExists(string commandName)
        {
            return _vaProxy?.CommandExists(commandName);
        }

        public static short? GetSmallInt(string sName)
        {
            return _vaProxy?.GetSmallInt(sName);
        }

        public static void SetSmallInt(string sName, short iValue)
        {
            _vaProxy?.SetSmallInt(sName, iValue);
        }

        public static int? GetInt(string sName)
        {
            return _vaProxy?.GetInt(sName);
        }

        public static void SetInt(string sName, int? iValue)
        {
            _vaProxy?.SetInt(sName, iValue);
        }

        public static bool? GetBoolean(string sName)
        {
            return _vaProxy?.GetBoolean(sName);
        }

        public static void SetBoolean(string sName, bool? bValue)
        {
            _vaProxy?.SetBoolean(sName, bValue);
        }

        public static DateTime? GetDate(string sName)
        {
            return _vaProxy?.GetDate(sName);
        }

        public static void SetDate(string sName, DateTime? dtValue)
        {
            _vaProxy?.SetDate(sName, dtValue);
        }

        public static decimal? GetDecimal(string sName)
        {
            return _vaProxy?.GetDecimal(sName);
        }

        public static void SetDecimal(string sName, decimal? dValue)
        {
            _vaProxy?.SetDecimal(sName, dValue);
        }

        public static void ExecuteCommand(string commandName, bool waitForReturn = false)
        {
            _vaProxy?.ExecuteCommand(commandName, waitForReturn);
        }

        public static void ExecuteStartupCommand()
        {
            _vaProxy?.ExecuteStartupCommand();
        }

        public static string GetProfileName()
        {
            return _vaProxy?.GetProfileName();
        }

        public static string GetText(string sName)
        {
            return _vaProxy?.GetText(sName);
        }

        public static void SetText(string sName, string sValue)
        {
            _vaProxy?.SetText(sName, sValue);
        }

        public static void WriteToLog(string valueToWrite, string color = "blank")
        {
            System.Diagnostics.Debug.WriteLine(valueToWrite);
            _vaProxy?.WriteToLog(valueToWrite, color);
        }
    }
}
