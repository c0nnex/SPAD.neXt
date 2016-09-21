using SPAD.neXt.Interfaces.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.GamePlugins.VoiceAttack
{
    class ServiceProxy : ClientBase<IRemoteService>, IRemoteService
    {
        public ServiceProxy(string hostname) : base(new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteService)),
            new NetNamedPipeBinding(), new EndpointAddress($"net.pipe://{hostname}/SPAD.neXt/RemoteService")))
        {

        }

        public bool IsConnected
        {
            get
            {
                switch (State)
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

        public bool TryToConnect()
        {
            switch (State)
            {
                case CommunicationState.Created:
                case CommunicationState.Closed:
                    {
                        Open();
                        return State == CommunicationState.Opened;
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

        public RemoteServiceResponse EmulateEvent(string eventTarget, string eventName, string eventParameter)
        {
            try
            {
                return Channel.EmulateEvent(eventTarget, eventName, eventParameter);
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
                return Channel.GetValue(variableName);
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
                return Channel.SetValue(variableName, newValue);
            }
            catch (Exception ex)
            {
                return new RemoteServiceResponse { HasError = true, Error = ex.Message };
            }
        }
    }
}
