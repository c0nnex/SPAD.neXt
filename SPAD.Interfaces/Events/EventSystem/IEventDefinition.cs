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

    public interface IEventBaseObject : IIsDeletable, IIsEditable
    {
        IEventDefinition GetParentEventDefinition();
        IDataDefinition GetDataDefinition(string definitionID);

        void InitalizeNewAction(IDeviceProfile deviceProfile, IEventDefinition eventDefinition);
    }

    public interface IEventDefinitions : IObservableList<IEventDefinition>, ICloneableWithID<IEventDefinitions>, IConditionalSerialize
    {
        string BoundTo { get; set; }
        string ConfigString { get; }
        string Name { get; set; }
        bool DebugMode { get; set; }
        void Configure(ISPADBaseEvent baseEvent, IDeviceProfile deviceProfile);
        void Activate(string boundTo, ISPADBaseEvent baseEvent);
        void Deactivate(string boundTo, ISPADBaseEvent baseEvent);
        IReadOnlyList<IEventDefinition> GetByTrigger(string trigger, bool create = true);
        IEventDefinition CreateEvent(string trigger, Guid? singletonID = null);
        IEventDefinition GetByID(Guid eventDefinitionID);
        void RemoveAll(IEnumerable<IEventDefinition> enumerable);
    }
    public interface IEncoderAcceleration
    {

        Double Threshold { get; set; }
        Double Timeout { get; set; }
        Double Multiplier { get; set; }
        Double Max { get; set; }

    }

    public interface IEventDefinition : IEventOptions, ICloneableWithID<IEventDefinition>, IConditionalSerialize, IIsDeletable, IProgrammableHeld
    {
        Guid SingletonID { get; set; }
        bool IsSingleton { get; }
        IEventActions Actions { get; }
        string BoundTo { get; }
        SPADConditionBinding ConditionBinding { get; set; }
        bool ConditionRepeat { get; set; }
        IEventConditions Conditions { get; }
        string ConfigString { get; }
        string ConfigStringToolTip { get; }
        string EventName { get; }
        bool IsFinal { get; set; }
        bool DebugMode { get; set; }
        string Comment { get; set; }
        string DisplayName { get; }
        string Trigger { get; set; }
        int Priority { get; }
        bool IgnorePowerState { get; }
        bool EnableAcceleration { get; }
        bool IsDisabled { get; set; }
        bool IsEnabled { get; set; }
        bool IsDeprecated { get; set; }
        bool HasWarning { get; }
        bool EventIsRunning { get; }
        string WarningMessage { get; }
        IEncoderAcceleration Acceleration { get; }

        InputModifier InputBehavior { get; set; }
        ISPADBaseEvent BaseEvent { get; }
        IDeviceProfile DeviceProfile { get; }

        void Configure(IEventDefinitions eventDefinitions, ISPADBaseEvent baseEvent, IDeviceProfile deviceProfile);
        void Deactivate(ISPADBaseEvent baseEvent);
        void Execute(ISPADEventArgs e, bool force = false);
        void SetDisplayName(string displayNameResource);
        void SetVariableNameTransformFunc(Func<IEventDefinition, string, string> transformFunc);
        string TransformVariableName(string varName);

        // Creating new actions
        void AddAction(IEventAction action, bool singleton = true);
        IEventAction CreateCommandAction(string command, string parameter = null);

        bool IsAction(SPADEventActions actionType);
        IEventAction GetFirstAction(SPADEventActions actionType);

    }

    public interface IEventCondition : IEventBaseObject, IHasID, ICloneable<IEventCondition>
    {
        string ConfigString { get; }
        bool IsConfigured { get; }
        bool LastEvalResult { get; }
        void SetDirty();
        void Activate();
        void Deactivate();
        void Update();
        bool CheckConfiguration(List<string> tmpError);
        bool Evaluate(ISPADEventArgs e);
    }

    public interface IEventEventCondition : IEventCondition
    {
        string BoundEvent { get; set; }
    }

    public interface IEventConditionExpression : ICloneable<IEventConditionExpression>, IEventCondition, IIsMonitorable
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
        ActionReferenceTypes ConditionType { get; set; }
        IReadOnlyList<IDataDefinition> ObservedDataDefinitions { get; }
        IEventConditionSimple SelfProperty { get; }

        void UpdateSelfProperty();
    }

    public interface ITemplateImplementation
    {
        string IdentityString { get; }
        string ConfigString { get; }
        string ConfigTooltip { get; }
        IExecutionCounter ExecutionCounter { get; }
    }

    public interface ITemplateClass : ISupportsActivation,ICloneableWithID<ITemplateClass>, IIsObservable
    {      
        string Name { get; set; }
        string Category { get; set; }
        bool ApplyTemplate(object target);
        int ApplyTemplateToProfile(IProfile profile);
        void FixUp();
        void UpdateTemplate(ITemplateClass newTemplate);
        ITemplateImplementation Implementation { get; }
        int NumberOfReferences { get; }
       // void AddReference();
    }

    public interface ITemplateable
    {
        bool CanCreateTemplate();
        ITemplateClass CreateTemplate(string templateName,string category = null);
    }

    public interface ISupportsActivation
    {
        void Activate();
        void Deactivate();

    }

    public interface IExecutionCounter
    {
        int Failure { get; }
        int Success { get; }
        int Total { get; }
        bool LastResult { get; }
        void RegisterResult(bool success);
    }

    public interface IIsEquatable
    {
        bool IsEqualTo(string otherIdentity);
    }

    public interface IEventConditions : IObservableList<IEventCondition>, ICloneable<IEventConditions>,IIsEquatable,ITemplateable, ITemplateImplementation //, IIsObservable
    {
        SPADConditionBinding ConditionBinding { get; set; }
        bool Evaluate(ISPADEventArgs e, SPADConditionBinding binding, bool debugMode);
        void SetConfiguration(SPADConditionBinding conditionBinding, bool conditionRepeat);
    }

    public interface IEventActions : IObservableList<IEventAction>
    {
        string ConfigString { get; }

        bool Execute(IEventDefinition definition, ISPADEventArgs e);

        IEventAction GetBySingleton(Guid id);

    }

    public interface IEditableObjectEx
    {
        int RenderSize { get; set; }
        void BeginEdit(IDeviceProfile deviceProfile);

        void EndEdit(IDeviceProfile deviceProfile);

        void CancelEdit(IDeviceProfile deviceProfile);
        Action<IEventAction, Action<object>> EditModeRenderCallBack { get; set; }
        void EditModeRender(object value, Action<object> renderMeCallback);
    }

    public interface IEventAction : INotifyPropertyChanged, IEventBaseObject, IEventOptions, ICloneableWithID<IEventAction>, IConditionalSerialize, IEditableObjectEx
    {
        SPADEventActions ActionID { get; }
        string ConfigID { get; }
        string ConfigString { get; }
        string Documentation { get; }
        string ActionHash { get; }
        bool IsDirty { get; set; }
        bool IsNew { get; }
        bool CanBeDeleted { get; set; }
        bool CanTargetDevice { get; set; }
        bool CanTargetSwitch { get; set; }
        string TargetDeviceID { get; set; }
        string RawTargetDeviceID { get; }
        string TargetSwitchName { get; set; }
        string DataReferenceText { get; }
        bool IsInEditMode { get; }
        void AddParserValue(string key, string value);
        int TransformValue(AxisEventValue e);
        bool Execute(IEventDefinition definition, ISPADEventArgs e);
        bool Equals(IEventAction other);
        bool CheckConfiguration(List<string> errorContainer);

        IDeviceProfile GetTargetDevice(IEventDefinition def);
        void SetEventValueCallback(string valueName, IEventValueCallback callback);
        void SetEventTargetCallback(string targetName, IEventTargetCallback callback);

        void SetConfiguration(string configID);
    }

    public interface IEventValueCallback
    {
        object GetEventValue(string name);
    }

    public interface IEventTargetCallback
    {
        string GetEventTargetName(string name);
    }

    public interface IEventContext
    {
        ISPADBaseEvent EventContext { get; }
        IEventDefinition EventDefintionContext { get; set; }
    }

    public interface IEventOptions
    {
        T GetOption<T>(string optionName, T defaultValue = default(T));
        bool HasOption(string optionName);
        void SetOption(string optionName, object value);

    }
}