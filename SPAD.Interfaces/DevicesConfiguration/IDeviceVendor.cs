using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IDeviceVendor
    {
        bool ContainsDevice(string productID);
        IDeviceConfiguration GetDevice(int productID);
        IDeviceConfiguration GetDevice(string productID);
        string Name { get; }
        IReadOnlyList<IDeviceConfiguration> Devices { get; }
        string VendorID { get; }
        ushort VendorIDAsInt { get; }
    }
}
