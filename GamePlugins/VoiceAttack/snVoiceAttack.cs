using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{
    /// <summary>
    /// VoiceAttack SPAD.neXt plugin
    /// </summary>
    class snVoiceAttack
    {

        public static string VA_DisplayName()
        {
            return "SPAD.neXt VoiceAttack Plugin - v0.9.5+";
        }

        public static string VA_DisplayInfo()
        {
            return "SPAD.neXt VoiceAttack Plugin\r\n\r\n2016 FSGS.com";
        }

        public static Guid VA_Id()
        {
            return new Guid("{B8FC149B-3107-4411-AC46-E0E20BC93460}");
        }

        public static void VA_Init1(ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            if (state.ContainsKey("VA_PROXY"))
            {
                dynamic vaproxy = state["VA_PROXY"];
                snVoiceAttackCommandInterface.VA_Init1(vaproxy);
            }
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
            snVoiceAttackCommandInterface.VA_Exit1(null);
        }

        public static void VA_StopCommand()
        {
            snVoiceAttackCommandInterface.VA_StopCommand();
        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            bool bDebug = vaProxy.IsDebug;
           
            try
            {
                object tmp;

                textValues["snSTATUS"] = "OK";
                textValues["snMESSAGE"] = String.Empty;
                if (bDebug) vaProxy.WriteToLog($"Command '{context}'");
                var proxy = snVoiceAttackCommandInterface.GetServiceProxy();
                if ((proxy == null) || !proxy.IsConnected)
                {
                    textValues["snSTATUS"] = "ERROR";
                    textValues["snMESSAGE"] = "No Connection";
                    return;
                }
                switch (context.ToLowerInvariant())
                {
                    case "getvalue":
                        {
                            string varName = decimalValues.Keys.FirstOrDefault();
                            if (string.IsNullOrEmpty(varName))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "GetValue: no decimal given";
                                return;
                            }
                            var result = proxy.GetValue(varName);
                            if (result.HasError)
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = $"GetValue: Failed to get {varName} : {result.Error}";
                                return;
                            }
                            if (bDebug) vaProxy.WriteToLog( String.Format("GotValue: {0} {1}", result.Value, Convert.ToDecimal(result.Value)));
                            decimalValues[varName] = Convert.ToDecimal(result.Value);
                            return;
                        }
                    case "setvalue":
                        {
                            string varName = decimalValues.Keys.FirstOrDefault(); ;
                            decimal? newValue;
                            if (string.IsNullOrEmpty(varName))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "SetValue: no decimal given";
                                return;
                            }
                            if (!decimalValues.TryGetValue(varName, out newValue) || !newValue.HasValue)
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = $"SetValue: {varName} has no decimal value";
                                return;
                            }
                            var result = proxy.SetValue(varName, Convert.ToDouble(newValue.GetValueOrDefault(0)));
                            if (result.HasError)
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = $"SetValue: Failed to set {varName} to {newValue} : {result.Error}";
                                return;
                            }
                            return;
                        }
                    case "executecommand": break;
                    case "emulateevent":
                        {
                            string snDevice, snSwitch, snEvent, snParameter;

                            if (!textValues.TryGetValue("snDevice", out snDevice) || string.IsNullOrEmpty(snDevice))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "EmulateEvent: snDevice not set";
                                return;
                            }
                            if (!textValues.TryGetValue("snSwitch", out snSwitch) || string.IsNullOrEmpty(snSwitch))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "EmulateEvent: snSwitch not set";
                                return;
                            }
                            if (!textValues.TryGetValue("snEvent", out snEvent) || string.IsNullOrEmpty(snEvent))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "EmulateEvent: snEvent not set";
                                return;
                            }
                            textValues.TryGetValue("snEventParameter", out snParameter);

                            var result = proxy.EmulateEvent(snDevice, snSwitch, snEvent, snParameter);
                            if (result.HasError)
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = $"EmulateEvent: {result.Error}";
                            }
                            return;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                textValues["snSTATUS"] = "ERROR";
                textValues["snMESSAGE"] = $"FATAL: {ex.Message}";
                if (bDebug) vaProxy.WriteToLog(ex.ToString());
            }
            finally
            {
                if (bDebug)
                {
                    vaProxy.WriteToLog($"snStaus='{textValues["snSTATUS"]}' snMessage='{textValues["snMESSAGE"]}' ");
                }
            }
        }
    }
}
