using System;
using System.Collections.Generic;
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
        }

        public static void VA_Exit1(ref Dictionary<string, object> state)
        {
        }

        public static void VA_Invoke1(String context, ref Dictionary<string, object> state, ref Dictionary<string, Int16?> shortIntValues, ref Dictionary<string, string> textValues, ref Dictionary<string, int?> intValues, ref Dictionary<string, decimal?> decimalValues, ref Dictionary<string, Boolean?> booleanValues, ref Dictionary<string, object> extendedValues)
        {
            switch (context)
            {
                case "GetValue": break;
                case "SetValue": break;
                case "ExecuteCommand": break;
                case "EmulateEvent": break;
                default:
                    break;
            }
        }
    }
}
