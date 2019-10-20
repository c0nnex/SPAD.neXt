using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface ISpecificOptions
    {
        Guid ID { get; }
        object GetVariable(string variableName);
        void UpdateVariable(string variableName, object value);
        bool HasVariable(string variableName);
        void AddVariable(string variableName, object value, bool overwriteExisting = false);
        void Save();
    }
}
