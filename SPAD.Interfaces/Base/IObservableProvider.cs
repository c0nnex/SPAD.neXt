using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface IObservableProvider<T> : IObservable<T>, IDisposable where T : IHandledEventArgs
    {
        void NotifyObservers(T arg);
    }
}
