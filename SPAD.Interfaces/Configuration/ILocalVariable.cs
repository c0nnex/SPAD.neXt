using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Configuration
{
    public interface ILocalVariable
    {
        string Name { get;  }
        string DisplayName { get; }
        object Value { get; }
        VARIABLE_SCOPE Scope { get; }
        bool IsMonitored { get; }
        bool IsReadOnly { get; }
        ValueDataTypes ValueDataType { get; }
    }

    public interface IVariablesProvider
    {
        void RemoveVariable(string name);
        bool HasVariable(string name);
        T GetVariable<T>(string name, T defaultValue = default);
        void SetVariable(string name, object value);
    }
}
