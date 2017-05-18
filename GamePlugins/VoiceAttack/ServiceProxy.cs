using Microsoft.Win32;
using SPAD.neXt.Interfaces.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{

    class RemoteServiceProxy : ClientBase<IRemoteService>
    {
        public RemoteServiceProxy(InstanceContext ctx, ServiceEndpoint ep) : base(ctx,ep)
        { }

        public IRemoteService RemoteChannel { get { return Channel; } }
    }

    class ServiceProxy :IRemoteService , IRemoteServiceCallback
    {
        protected RemoteServiceProxy prxy;

        public event EventHandler<string> RemoteEventReceived;
        
        public ServiceProxy(string hostname) 
        {
            prxy = new RemoteServiceProxy(new InstanceContext(this), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteService)),
            new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), new EndpointAddress($"net.pipe://localhost/SPAD.neXt/RemoteService")));
        }

        public bool IsConnected
        {
            get
            {
                switch (prxy.State)
                {
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                    case CommunicationState.Faulted:
                        return false;
                    case CommunicationState.Opened:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public void Close()
        {
            try
            {
                prxy?.Close();
            }
            catch { }
        }

        public bool TryToConnect()
        {
            try
            {
                switch (prxy.State)
                {
                    case CommunicationState.Created:
                    case CommunicationState.Closed:
                        {
                            int tries = 0;
                            while ((prxy.State != CommunicationState.Opened) && (tries < 20))
                            {
                                prxy.Open();
                                Thread.Sleep(100);
                                tries++;
                            }
                            if ( prxy.State == CommunicationState.Opened )
                            {
                                switch (Initialize(null, null))
                                {
                                    case RemoteServiceResult.CONNECTION_OK:
                                        return true;
                                    case RemoteServiceResult.CONNECTION_OUTDATED:
#if !STANDALONE
                                        vaProxy.WriteToLog("SPAD.neXt VA Plugin outdated! please update!", "red");
#endif
                                        return false;
                                    case RemoteServiceResult.CONNECTION_DENIED:
                                    case RemoteServiceResult.CONNECTION_BUSY:
#if !STANDALONE
                                        vaProxy.WriteToLog("Connection to SPAD.neXt failed.");
#endif
                                        return false;
                                    default:
                                        return false;
                                }
                            }
                            return false;
                        }
                    case CommunicationState.Closing:
                    case CommunicationState.Opening:
                    case CommunicationState.Faulted:
                        return false;
                    case CommunicationState.Opened:
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
#if !STANDALONE
                vaProxy.WriteToLog(ex.Message, "red");
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
                return prxy.RemoteChannel.EmulateEvent(targetDevice, targetEvent, eventTrigger, eventParameter);
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

                return prxy.RemoteChannel.GetValue(variableName);
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

                return prxy.RemoteChannel.SetValue(variableName, newValue);
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }

        public RemoteServiceResponse Monitor(string variableName)
        {
            try
            {
                if (!TryToConnect())
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };

                return prxy.RemoteChannel.Monitor(variableName);
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }


        public void RemoteEvent(string eventName,string value)
        {
            RemoteEventReceived?.Invoke(this, eventName);
        }

        public string GetVersion()
        {
            try
            {
                if (!TryToConnect())
                    return null;

                return prxy.RemoteChannel.GetVersion();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Ping(ulong tick)
        {
            try
            {
                if (!TryToConnect())
                    return;
                uint x = (uint)Environment.TickCount;
                prxy.RemoteChannel.Ping(x);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void Pong(ulong tick)
        {
            ulong x = (ulong)EnvironmentEx.TickCount;
            vaProxy.WriteToLog($"PingPong Out={tick} In={x} RoundTrip={x - tick}");
        }

        public RemoteServiceResult Initialize(string clientName, Version remoteApiVersion)
        {
            try
            {
                if (!TryToConnect())
                    return RemoteServiceResult.CONNECTION_DENIED;

                return prxy.RemoteChannel.Initialize("VoiceAttack", RemoteServiceContract.RemoteApiVersion);
            }
            catch (Exception ex)
            {
                return RemoteServiceResult.CONNECTION_DENIED;
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
