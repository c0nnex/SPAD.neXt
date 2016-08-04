using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IKnownDevices
    {
        IDeviceConfiguration Find(string vendorID, string productID);
        IReadOnlyList<IDeviceVendor> Vendors { get; }
    }
}
