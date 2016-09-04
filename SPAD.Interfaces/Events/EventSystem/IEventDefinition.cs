using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SPAD.neXt.Interfaces.Events
{
    public interface IEventDefinitions : IObservableList<IEventDefinition>, ICloneableWithID<IEventDefinitions>
    {
        string BoundTo { get; set; }
        string ConfigString { get; }
        string Name { get; set; }

        void Configure(ISPADBaseEvent baseEvent, IDeviceProfile deviceProfile);
        void Activate(string boundTo, ISPADBaseEvent baseEvent);
        void Deactivate(string boundTo, ISPADBaseEvent baseEvent);
        void Execute(ISPADEventArgs e);
        IReadOnlyList<IEventDefinition> GetByTrigger(string trigger, bool create = true);
        IEventDefinition CreateEvent(string trigger);
        IEventDefinition GetByID(Guid eventDefinitionID);
    }

   
    public interface IEventDefinition : ICloneableWithID<IEventDefinition>
    {
        IEventActions Actions { get; }
        string BoundTo { get; }
        SPADConditionBinding ConditionBinding { get; set; }
        IEventConditions Conditions { get; }
        string ConfigString { get; }
        string ConfigStringToolTip { get; }
        string EventName { get; }
        bool IsFinal { get; set; }
        string Comment { get; set; }
        string DisplayName { get; }
        string Trigger { get; set; }
        int Priority { get; }
        bool IgnorePowerState { get; }
        ISPADBaseEvent BaseEvent { get; }
        InputModifier InputBehavior { get; set; }

        void Configure(IEventDefinitions eventDefinitions, ISPADBaseEvent baseEvent, IDeviceProfile deviceProfile);
        bool Activate(ISPADBaseEvent baseEvent);
        void Deactivate(ISPADBaseEvent baseEvent);
        void Execute(ISPADEventArgs e, bool force = false);
        void SetBinding(string boundTo);
        void SetDisplayName(string displayNameResource);
        // Creating new actions
        void AddAction(IEventAction action, bool singleton = true);
        IEventAction CreateCommandAction(string command, string parameter = null);
    }

    public interface IEventCondition : IHasID
    {
        IEventDefinition ParentEventDefinition { get; }
        string ConfigString { get; }
        bool IsConfigured { get; }

        void Activate();
        void Deactivate();
        bool CheckConfiguration(List<string> tmpError);
        bool Evaluate(ISPADEventArgs e);
    }

    public interface IEventConditionExpression : ICloneable<IEventConditionExpression>,IEventCondition,IIsMonitorable
    {
        string Expression { get; set; }
    }

    public interface IEventConditionSimple : ICloneableWithID<IEventConditionSimple>, IEventCondition
    {
        SPADEventValueComparator ConditionComparator { get; set; }

        bool ConditionValueSourceEnabled { get; }
        IDataDefinition ConditionValueSource { get; set; }
        string ConditionValueSourceString { get; set; }

        bool ConditionTargetValueEnabled { get; }
        string ConditionTargetValue { get; set; }
        IDataDefinition ConditionTargetValueSource { get; set; }
        string ConditionTargetValueID { get; set; }
        
        IEventConditionSimple SelfProperty { get; }

        void UpdateSelfProperty();
    }

    public interface IEventConditions : IObservableList<IEventCondition>
    {
        string ConfigString { get; }
        bool Evaluate(ISPADEventArgs e, SPADConditionBinding binding);
    }

    public interface IEventActions : IObservableList<IEventAction>
    {        
        string ConfigString { get; }

        void Execute(IEventDefinition definition, ISPADEventArgs e);
    }

    public interface IEventAction : INotifyPropertyChanged , ICloneableWithID<IEventAction>
    {
        IEventDefinition ParentEventDefinition { get; }
        SPADEventActions ActionID { get; }
        string ConfigID { get; }
        string ConfigString { get; }
        string ActionHash { get; }
        bool IsDirty { get; set; }
        bool IsNew { get; }
        bool CanBeDeleted { get; set; }
        bool CanTargetDevice { get; set; }
        string TargetDeviceID { get; set; }
        string TargetName { get; set; }

        T GetOption<T>(string optionName, T defaultValue = default(T));
        bool HasOption(string optionName);
        void SetOption(string optionName, object value);
        int TransformValue(AxisEventValue e);
        void Execute(IEventDefinition definition, ISPADEventArgs e);
        bool Equals(IEventAction other);
        bool CheckConfiguration(List<string> errorContainer);
        
        IDeviceProfile GetTargetDevice(IEventDefinition def);
    }

    public interface IEventContext
    {
        ISPADBaseEvent EventContext { get; }
        ObservableCollection<IEventDefinition> EventDefinitions { get; }
        IEventDefinition EventDefintionContext { get; set; }
        string EventDisplayName { get; }
        string EventName { get; }
        IInput Input { get; }
        bool IsConfigured { get; }
    }
}