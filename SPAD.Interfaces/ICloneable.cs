using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface ICloneableWithID<T> : IDisposable,IHasID, ICloneable<T> where T : class
    {
    }

    public interface IHasID 
    {
        Guid ID { get; }
        void ChangeID(Guid newID);
    }

    public interface ICloneable<T> where T : class
    {
        T Clone();
    }

    public interface IObservableList<T> : IList<T>
    {
        void Replace(T oldItem, T newItem);
    }
   
}
