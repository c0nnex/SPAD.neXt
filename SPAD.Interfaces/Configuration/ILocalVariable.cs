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
        double Value { get; }
        VARIABLE_SCOPE Scope { get; }
        bool IsMonitored { get; }
        bool IsReadOnly { get; }
    }
}
