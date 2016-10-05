using SPAD.neXt.Interfaces.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
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
        static Version CurrentRemoteServiceVersion = new Version("1.1.0.0");
        private RemoteServiceProxy prxy;

        public event EventHandler<string> RemoteEventReceived;
        
        public ServiceProxy(string hostname) 
        {
            prxy = new RemoteServiceProxy(new InstanceContext(this), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteService)),
            new NetNamedPipeBinding(), new EndpointAddress($"net.pipe://localhost/SPAD.neXt/RemoteService")));
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
                            return prxy.State == CommunicationState.Opened;
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
                vaProxy.WriteToLog(ex.Message, "red");
                return false;
            }
        }

        public RemoteServiceResponse EmulateEvent(string eventTarget, string eventName, string eventParameter)
        {
            try
            {
                if ( ! TryToConnect() )
                    return new RemoteServiceResponse { HasError = true, Error = "No Connection" };
                return prxy.RemoteChannel.EmulateEvent(eventTarget, eventName, eventParameter);
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

        public List<RemoteEventTarget> GetEventTargets()
        {
            throw new NotImplementedException();
        }

        public void RemoteEvent(string eventName)
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
    }
}
