using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.HID
{
    public interface IUSBDevice
    {
        string VendorHexId { get; }
        ushort VendorId { get; }
        string ProductHexId { get; }
        ushort ProductId { get; }
        string DevicePath { get; }
        string SerialNumber { get; }
        string Version { get; }
        string Description { get; set; }
        string InstanceId { get; }

        Guid SessionDeviceID { get; }
        string DeviceKey { get; }
        string ProductIndex { get; }
        string DeviceProfileID { get; }
        Guid DirectInputInterfaceID { get; set; }
        int DeviceSystemNumber { get; set; }
        int DeviceIndex { get; set; }
        string ProductName { get; set; }
        string ManufacturerName { get; set; }
        void SetDeviceKey(string devicePath);
    }

    
}
