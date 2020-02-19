using System;

namespace SPAD.neXt.Interfaces.Gauges
{

    public enum GaugeVersionStatus
    {
        OK,
        UPDATE_AVAILABLE,
        OUTDATED
    };

    public interface IGaugeVersionInformation
    {
        Version CurrentVersion { get; }
        Guid Guid { get; }
        string Name { get; }
        string UpdateURL { get; }
    }
}