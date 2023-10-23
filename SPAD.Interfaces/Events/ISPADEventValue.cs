using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Events
{

    public interface ISPADEventDelegate
    {
        bool NeedNotify { get; }
        void OnMonitoredValueChanged(string eventName,IMonitorableValue sender, ISPADEventArgs e);
    }


    public interface ISPADComparator
    {
        IComparable compareValueLeft { get;  }
        IComparable compareValueRight { get; }
        bool DoesMatch(IComparable testValue);
    }


}
