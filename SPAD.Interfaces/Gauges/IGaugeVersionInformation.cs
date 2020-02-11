using System;

namespace SPAD.neXt.Interfaces.Gauges
{
    public interface IGaugeVersionInformation
    {
        Version CurrentVersion { get; }
        Guid Guid { get; }
        string Name { get; }
        string UpdateURL { get; }
    }
}