using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface ISerializableOptionBase : ICloneable
    {
        string Key { get; set; }
        T GetValue<T>();
        bool SetValue(object value);
    }

    public interface ISerializableOption : ISerializableOptionBase
    {       
        bool AsBoolean { get; set; }
        int AsInt { get; set; }
        Double AsDouble { get; set; }
        string AsString { get; set; }
       
    }

    public interface IOptionsProvider
    {
        bool HasOption(string optionName);
        TOPT GetOption<TOPT>(string optionName, TOPT defaultValue = default(TOPT));
        bool SetOption(string optionName, object value);
        void RemoveOption(string optionName);
    }

    public interface ISerializableList
    {
        ISerializableOptionBase this[string key] { get; set; }

        IReadOnlyList<string> Keys { get; }

        bool ContainsKey(string key);
        TOPT GetOption<TOPT>(string optionName, TOPT defaultValue = default(TOPT));
        bool HasOption(string optionName);
        bool Merge(IReadOnlyList<ISerializableOption> other);
        void Remove(string key);
        bool SetOption(string optionName, object value);
    }
}
