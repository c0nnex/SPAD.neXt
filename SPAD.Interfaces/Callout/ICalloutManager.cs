using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Callout
{    

    public interface ICallout
    {
        long CalloutID { get; }
        DateTime When { get; }
        TimeSpan Delay { get; }
        bool Done { get; set; }
        bool Persistent { get; }       
        object Owner { get; }  
        object Parameter { get; }

        void EndSchedule();
        void Reschdedule();
    }
    
    public interface ICalloutManager
    {
        ICallout AddCallout(object owner, EventHandler<ICallout> callback, object parameter , TimeSpan delay, bool persistant);
        void RemoveCallout(ICallout callout);
        void RemoveAllCalloutsFor(object owner);
    }
}
