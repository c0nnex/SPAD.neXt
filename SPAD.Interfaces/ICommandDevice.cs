using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface ICommandDevice
    {

    }

    public interface IActionProvider
    {
        Guid ID { get; }
        string ActionName { get; }

        IActionConfigurationControl GetActionConfigurationControl(IActionConfiguration actionConfiguration);
        string GetActionConfigString(IEventActionExternal action);
        void Activate(IEventActionExternal action);
        bool Execute(IEventActionExternal action);
        void Deactivate(IEventActionExternal action);

        IReadOnlyList<IDataDefinition> GetMonitoredDataDefinitions(IEventActionExternal action);
    }

    public interface IActionConfiguration
    {
        IEventAction ActionEvent { get; }
        ISPADBaseEvent BaseEvent { get;  }
        IDeviceSwitch DeviceSwitch { get;  }
        IDeviceConfigValue SwitchConfiguration { get;}
        IEventDefinition EventDefinition { get; }
        IDeviceProfile DeviceProfile { get;  }
        IInput AttachedInput { get; }
        bool IsNewAction { get; }
        object ParameterObj { get; }
    }

    public interface IActionConfigurationControl 
    {
        bool OnCanSave();

        void Initialize(IActionConfiguration actionConfiguration);
        void SaveChanges();
    }
}
