using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IObjectWithOptions
    {
        void AddOption(string key, object value, int pos = -1);
        I GetOption<I>(string key, I defaultValue = default) where I : IConvertible;
        bool HasOption(string key);
        int RemoveOption(string key);
        void SetOption<I>(string key, I value) where I : IConvertible;
    }
    public interface IExtensible
    {
        T GetExtension<T>(Type type) where T : IXmlAnyObject;
        bool HasExtension<T>() where T : IXmlAnyObject;

    }

    public interface ITransferable
    {
        string Export();
        T Import<T>(string data);
    }
    public interface ICloneableWithID<T> : IDisposable,IHasID, ICloneable<T> where T : class
    {
    }

    public interface IHasID 
    {
        Guid ID { get; }
        void ChangeID(Guid newID);
    }

    public interface ICustomCloneable<T> : ICloneable<T> where T : class
    {
        T Clone(Action<T> customize = null);
    }

    public interface ICloneable<T> where T : class
    {
        T Clone();
    }

    public interface IObservableList<T> : IList<T>
    {
        void Replace(T oldItem, T newItem);
    }

    public interface IObservableReadOnlyList<T> : IReadOnlyList<T>
    {
        void Replace(T oldItem, T newItem);
        void Remove(T oldItem);
    }

}
