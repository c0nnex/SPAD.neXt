using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.UI
{
    [Obsolete]
    public interface IDialConfigurationDialog
    {
        void SetDialConfig(int dialNumber, string labelRessource, string eventValueName, bool isEventValue, string defaultValue);
        void DisableDialConfig(int dialNumber);
        void SetDialMinMax(int dialNumber, Double min, Double max, bool reverse);
    }
}
