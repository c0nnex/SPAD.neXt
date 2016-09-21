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
            textValues["snHostname"] = "localhost";
            textValues["snSTATUS"] = "OK";
            textValues["snMESSAGE"] = String.Empty;
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
            object tmp;
            if (state.TryGetValue("snRemoteService", out tmp))
            {
                ServiceProxy proxy = tmp as ServiceProxy;
                if ((proxy != null) && (proxy.IsConnected))
                    proxy.Close();
                state["snRemoteService"] = null;
            }
        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            try
            {
                object tmp;
                ServiceProxy proxy = null;

                textValues["snSTATUS"] = "OK";
                textValues["snMESSAGE"] = String.Empty;
                if (!state.ContainsKey("snRemoteService"))
                {
                    state["snRemoteService"] = new ServiceProxy("localhost"); // textValues["snHostname"]);
                }
                if (state.TryGetValue("snRemoteService", out tmp))
                {
                    proxy = tmp as ServiceProxy;
                    if ((proxy != null) && (!proxy.TryToConnect()))
                    {
                        proxy = null;
                    }
                }
                if (proxy == null)
                {
                    textValues["snSTATUS"] = "ERROR";
                    textValues["snMESSAGE"] = "Not Connected";
                    return;
                }

                switch (context.ToLowerInvariant())
                {
                    case "getvalue":
                        {
                            string varName = decimalValues.Keys.FirstOrDefault();
                            if (!string.IsNullOrEmpty(varName))
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
                            Debug.WriteLine("GotValue: {0} {1}", result.Value, Convert.ToDecimal(result.Value));
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
                            string eventName, eventTarget, eventParameter;

                            if (!textValues.TryGetValue("snEventTarget", out eventTarget) || string.IsNullOrEmpty(eventTarget))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "EmulateEvent: snEventTarget not set";
                                return;
                            }
                            if (!textValues.TryGetValue("snEventName", out eventName) || string.IsNullOrEmpty(eventName))
                            {
                                textValues["snSTATUS"] = "ERROR";
                                textValues["snMESSAGE"] = "EmulateEvent: snEventName not set";
                                return;
                            }
                            textValues.TryGetValue("snEventParameter", out eventParameter);

                            var result = proxy.EmulateEvent(eventTarget, eventName, eventParameter);
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
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
