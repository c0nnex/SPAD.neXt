using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{
    /// <summary>
    /// VoiceAttack SPAD.neXt command interface
    /// </summary>
    internal static class snVoiceAttackCommandInterface
    {
#if OLD_V4
        public static string VA_DisplayName()
        {
            return "SPAD.neXt command interface (DO NOT USE)";
        }

        public static string VA_DisplayInfo()
        {
            return "SPAD.neXt command interface\r\n\r\n2016 FSGS.com";
        }

        public static Guid VA_Id()
        {
            return new Guid("{52AAC086-D5E5-4655-8873-805454C4EE23}");
        }

        public static void VA_Invoke1(dynamic vaproxy)
        { }
#endif
        public static string PluginVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        public static void VA_StopCommand()
        {
            vaProxy.WriteToLog($"SPAD.neXt command interface StopCommand", "blank");
            isStopped = true;
            proxy?.Close();
            proxy = null;
            vaProxy.SetVAProxy(null);
        }



        public static void VA_Init1(dynamic vaproxy)
        {
            vaProxy.SetVAProxy(vaproxy);
            vaProxy.WriteToLog($"SPAD.neXt command interface initialize (Plugin Version {PluginVersion})", "blank");
            InitProxy();

            vaProxy.SetText("snHostname", "localhost");
            vaProxy.SetText("snSTATUS", "OK");
            vaProxy.SetText("snMESSAGE", String.Empty);
        }

        public static void VA_Exit1(dynamic vaproxy)
        {
            if ((proxy != null) && (proxy.IsConnected))
            {
                proxy.RemoteEventReceived -= Proxy_RemoteEventReceived;
                proxy.Close();
                proxy = null;
            }
            vaProxy.WriteToLog($"SPAD.neXt command interface deinitialize", "blank");
            vaProxy.SetVAProxy(null);
        }

        

        private static ServiceProxy proxy = null;

        private static bool isStopped = false;

        private static void InitProxy()
        {
            if (isStopped)
                return;
            if (proxy == null)
            {
                proxy = new ServiceProxy("localhost");
                proxy.RemoteEventReceived += Proxy_RemoteEventReceived;
            }
            if (!proxy.IsConnected)
            {
                var version = proxy.GetVersion();
                if (!String.IsNullOrEmpty(version))
                {
                    vaProxy.WriteToLog($"SPAD.neXt connection established. Version {version}", "green");
                }
                else
                {
                    vaProxy.WriteToLog($"no SPAD.neXt connection", "red");
                }
            }
        }

        private static void Proxy_RemoteEventReceived(object sender, string eventName)
        {
            //    Debug.WriteLine($"Received RemoteEvent: {eventName}");
            vaProxy.WriteToLog($"Received RemoteEvent: {eventName}", "blank");
            if (vaProxy.CommandExists(eventName))
                vaProxy.ExecuteCommand(eventName);
        }

        public static ServiceProxy GetServiceProxy()
        {
            InitProxy();
            return proxy;
        }

 
    }
}
