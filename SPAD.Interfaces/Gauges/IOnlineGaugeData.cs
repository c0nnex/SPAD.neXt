using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Gauges
{
    public interface IOnlineGaugeData
    {
        string Crc { get;  }
        string CrcGauge { get;  }
        byte[] Data { get;  }
        string Filename { get;  }
        IOnlineGauge GaugeInfo { get;  }
        int Size { get;  }
    }
}
