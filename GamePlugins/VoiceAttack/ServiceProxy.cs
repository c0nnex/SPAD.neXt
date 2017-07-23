using Microsoft.Win32;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkingDataContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{

    class ServiceProxy 
    {
        static int DefaultTimeout = 5000;

        protected Connection Connection;
        protected string RemoteVersion = "unknown";
        public event EventHandler<string> RemoteEventReceived;
        
        public ServiceProxy(string hostname) 
        {
            NetworkComms.AppendGlobalConnectionEstablishHandler(OnConnectionEstablished, true);
            NetworkComms.AppendGlobalConnectionCloseHandler(OnConnectionClosed);
            NetworkComms.IgnoreUnknownPacketTypes = false;
            // NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions<NetworkingDataContracts.NetworkingDataContractSerializer>();
            
        }

        public virtual bool IsConnected
        {
            get
            {
                return ((Connection != null) && (Connection.ConnectionInfo.ConnectionState == ConnectionState.Established));
            }
        }

        public void Close()
        {
            try
            {
                Connection?.CloseConnection(false);
            }
            catch { }
        }

        private void OnConnectionClosed(Connection connection)
        {
            vaProxy.WriteToLog($"Connection Closed {connection?.ConnectionInfo?.RemoteEndPoint}","red");
            RemoteEventReceived?.Invoke(this, "SN_CONNECTION_CLOSED");
            Connection = null;
            
        }

        private void OnConnectionEstablished(Connection connection)
        {
           // logger.Info($"New Connection To {connection.ConnectionInfo.RemoteEndPoint}");

            var res = connection.SendReceiveObject<NetworkingDataContracts.NetworkConnect, NetworkingDataContracts.NetworkConnectResponse>(new NetworkingDataContracts.NetworkConnect() {
                ClientName = "VoiceAttack",
                RemoteApiVersion = NetworkingDataContracts.ContractConstants.RemoteApiVersion,
                ContractName = "VA", ContractVersion = Assembly.GetExecutingAssembly().GetName().Version
            });
            if ((res == null) || (res.Result != RemoteServiceResult.CONNECTION_OK))
            {
                switch (res.Result)
                {
                    case RemoteServiceResult.CONNECTION_OUTDATED:
#if !STANDALONE
                        vaProxy.WriteToLog("SPAD.neXt VA Plugin outdated! please update!", "red");
#endif
                        break;
                    case RemoteServiceResult.CONNECTION_DENIED:
                    case RemoteServiceResult.CONNECTION_BUSY:
#if !STANDALONE
                        vaProxy.WriteToLog("Connection to SPAD.neXt failed.","red");
#endif
                        break;
                    default:
                        vaProxy.WriteToLog($"Unknown Response: {res.Result}");
                        break;
                }
                connection.CloseConnection(false);
                return;
            }
            RemoteVersion = res.RemoteVersion;
            Connection = connection;
            connection.AppendIncomingPacketHandler<NetworkEvent>("RS.EVENT", OnRemoteEvent);
        }

        private void OnRemoteEvent(PacketHeader packetHeader, Connection connection, NetworkEvent incomingObject)
        {
            RemoteEventReceived?.Invoke(this, incomingObject.EventKey);
        }

        public bool TryToConnect()
        {
            try
            {
                if (IsConnected)
                    return true;
                TCPConnection.GetConnection(new ConnectionInfo("127.0.0.1", 8181), true);
                return IsConnected;
            }
            catch (Exception ex)
            {
#if !STANDALONE
                vaProxy.WriteToLog(ex.ToString(), "red");
#endif
                return false;
            }
        }

        public RemoteServiceResponse EmulateEvent(string targetDevice, string targetEvent, string eventTrigger, string eventParameter)
        {
            try
            {
                if ( ! TryToConnect() )
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };
                return Connection.SendReceiveObject<NetworkEvent, RemoteServiceResponse>("RS.EMULATEEVENT", "RemoteServiceResponse", DefaultTimeout,
                    new NetworkEvent(targetDevice, targetEvent, eventTrigger).WithData("PARAMETER", eventParameter));
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }

        public RemoteServiceResponse GetValue(string variableName)
        {
            try
            {
                if (!TryToConnect())
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };

                return Connection.SendReceiveObject<NetworkEvent, RemoteServiceResponse>("RS.GETVALUE", "RemoteServiceResponse", DefaultTimeout, NetworkEvent.Create(variableName));
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }

        public RemoteServiceResponse SetValue(string variableName, double newValue)
        {
            try
            {
                if (!TryToConnect())
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };

                return Connection.SendReceiveObject<NetworkEvent, RemoteServiceResponse>("RS.SETVALUE", "RemoteServiceResponse", DefaultTimeout, NetworkEvent.Create(variableName).WithEventTrigger(newValue.ToString(CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.ToString() };
            }
        }

        public RemoteServiceResponse Monitor(string variableName)
        {
            try
            {
                if (!TryToConnect())
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };

                return Connection.SendReceiveObject<NetworkEvent, RemoteServiceResponse>("RS.MONITOR", "RemoteServiceResponse", DefaultTimeout, NetworkEvent.Create(variableName));
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }

        public string GetVersion()
        {
            try
            {
                if (!TryToConnect())
                    return null;

                return RemoteVersion;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public static class EnvironmentEx
    {
        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        public static TimeSpan UpTime
        {
            get { return TimeSpan.FromMilliseconds(GetTickCount64()); }
        }

        public static UInt64 TickCount64
        {
            get { return GetTickCount64(); }
        }

        public static UInt64 TickCount
        {
            get { return GetTickCount64(); }
        }

        public static long TickCountLong
        {
            get { return (long)GetTickCount64(); }
        }

        public static string WindowsVersion
        {
            get
            {
                try
                {
                    using (var mainKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                    {
                        var val = mainKey.GetValue("ProductName");
                        if (val != null)
                            return val.ToString();
                    }
                    return "Unknown";
                }
                catch (Exception ex)
                { return "Unknown"; }
            }
        }

        public static bool IsWindows10
        {
            get { return WindowsVersion.ToLowerInvariant().StartsWith("windows 10"); }
        }
    }
}
