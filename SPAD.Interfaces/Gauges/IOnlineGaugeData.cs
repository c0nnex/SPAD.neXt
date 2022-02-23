using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Gauges
{
    public interface IOnlineGaugeData
    {
        string Crc { get; set; }
        string CrcGauge { get; set; }
        byte[] Data { get; set; }
        int ErrorCode { get; set; }
        string Filename { get; set; }
        IOnlineGauge GaugeInfo { get; set; }
        int Size { get; set; }
    }
}
