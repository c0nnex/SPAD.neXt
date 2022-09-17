using System;

namespace SPAD.neXt.Interfaces
{
    public sealed class SimulationConfiguration
    {
        public string SimulationName { get; set; }
        public Version SimulationVersion { get; set; }
        public Version InterfaceVersion { get; set; }
        public SimulationTypes SimulationType { get; set; } = SimulationTypes.UNKNOWN;
        public bool CanUseTaggedUpdates { get; private set; } = true;

        public SimulationConfiguration(string appName, Version appVersion, Version interfaceVersion)
        {
            SimulationName = appName;
            SimulationVersion = appVersion;
            InterfaceVersion = interfaceVersion;
           
        }
    }


}
