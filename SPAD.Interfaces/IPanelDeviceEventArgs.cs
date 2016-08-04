using SPAD.neXt.Interfaces.HID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IPanelDeviceEventArgs 
    {
        IPanelDeviceReport Report { get; }
    }

    public class PanelDeviceReport : IPanelDeviceReport
    {

        public PanelDeviceReport(IHidReport rep)
        {
            _Data = rep.GetBytes();
            _ReadStatus = (PanelDevicReadStatus)rep.ReadStatus;
            _ReportId = rep.ReportId;
            Tick = rep.Tick;
        }
        public UInt64 Tick { get; private set; }

        public byte[] Data
        {
            get { return _Data; }
        }
        private byte[] _Data;

        public PanelDevicReadStatus ReadStatus
        {
            get { return _ReadStatus; }
        }
        private PanelDevicReadStatus _ReadStatus;

        public byte ReportId
        {
            get { return _ReportId; }
        }
        private byte _ReportId;
    }

    public class PanelDeviceEventArgs : EventArgs, IPanelDeviceEventArgs
    {
        public PanelDeviceEventArgs(IPanelDeviceReport report)
        {
            Report = report;
        }
        public IPanelDeviceReport Report { get; private set; }
    }

}
