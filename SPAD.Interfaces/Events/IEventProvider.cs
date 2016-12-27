using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{
    public interface IEventProvider
    {
        IDisposable SubscribeToClientEvent<T>(string eventName, EventHandler<T> eventHandler);
       void RaisePanelEvent(string eventName, ISPADEventArgs e);
    }

}
