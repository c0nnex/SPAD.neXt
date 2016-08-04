using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IPanelDeviceReport
    {
        byte[] Data { get;  }
        PanelDevicReadStatus ReadStatus { get; }
        byte ReportId { get; }
        ulong Tick { get; }
    }
}
