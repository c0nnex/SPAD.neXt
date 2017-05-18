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
        double GetVariable(string variableName);
        void UpdateVariable(string variableName, double value);
        bool HasVariable(string variableName);
        void AddVariable(string variableName, double value, bool overwriteExisting = false);
        void Save();
    }
}
