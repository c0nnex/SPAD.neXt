using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.DevicesConfiguration;
using SPAD.neXt.Interfaces.Profile;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.Events
{
    
    public interface ISPADBaseEvent : IOptionsProvider,IXmlAnyObject,ICloneable<ISPADBaseEvent>
    {
        string BoundTo { get; }
        string ConfigID { get; set; }
        bool isActivated { get; }
        bool IsEnabled { get; set; }        
        bool CannotUpgradeToEventSystem { get; }
        bool IsGaugeEvent { get; }
        
        IEventDefinitions EventDefinitions { get; set; }
        IDeviceProfile LinkedDeviceProfile { get; }

        Guid EventExecutionContext { get; }
        void SetGaugeEvent(bool isGaugeEvent);

        void ActivateEvent(IDeviceProfile deviceProfile, Guid executionContext);
        void DeactivateEvent(IDeviceProfile deviceProfile);
        void Configure(IDeviceProfile deviceProfile);
        IReadOnlyList<IEventDefinition> GetByTrigger(string trigger);
        bool IsConfiguredEventValue(string valueName);
        bool IsConfigured();
        void RaiseEvent(ISPADEventArgs e);
        
        IDeviceSwitch GetSwitch();

        ISerializableOption GetOption(string key, string defaultVal = null);
        ISerializableOption GetPrivateOption(string key);
        bool HasPrivateOption(string key);
        string GetXML();
        void SetBinding(string bound);

        void EnableHeldMode(Guid owner);
    }

    
}
