using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{
    public interface IEventProvider
    {
        string DeviceEventProviderSerial { get; }
        Guid DeviceEventProviderID { get; }
        IDisposable SubscribeToClientEvent<T>(string eventName, EventHandler<string,T> eventHandler);
        void IncomingClientEvent(string eventName, ISPADEventArgs e);
        List<ISPADEventArgs> GetStartupEvents();
    }

}
