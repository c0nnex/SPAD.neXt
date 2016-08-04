using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IDeviceSwitchesType
    {
        IReadOnlyList<IDeviceSwitch> DeviceSwitch { get; }
    }
}
