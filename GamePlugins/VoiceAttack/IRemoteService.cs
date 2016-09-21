using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.ServiceContract
{
    public sealed class RemoteServiceResponse
    {
        public bool HasError { get; set; }
        public string Error { get; set; }
        public double Value { get; set; }
    }


    [ServiceContract]
    [ServiceKnownType(typeof(RemoteServiceResponse))]
    public interface IRemoteService
    {
        [OperationContract]
        RemoteServiceResponse GetValue(string variableName);

        [OperationContract]
        RemoteServiceResponse SetValue(string variableName, double newValue);

        [OperationContract]
        RemoteServiceResponse EmulateEvent(string eventTarget, string eventName, string eventParameter);
    }
}
