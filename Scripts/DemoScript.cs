using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Scripting;

namespace myScript
{
    public class myMultipanel : IScriptValueProvider
    {
        public double ProvideValue(IApplication app)
        {
            int radio_selected = (int)(EventSystem.GetDataDefinition("LOCAL:RADIO_SELECTED")?.GetValue()).GetValueOrDefault(0);
            IDataDefinition sourceVal = null;

            switch (radio_selected)
            {
                case 0: sourceVal = EventSystem.GetDataDefinition("SIMCONNECT:COM STANDBY FREQUENCY:1");break;
                case 1: sourceVal = EventSystem.GetDataDefinition("SIMCONNECT:COM ACTIVE FREQUENCY:1"); break;
                default: break;
            }
            return (sourceVal?.GetValue()).GetValueOrDefault(0) * 100;
        }
    }
}
