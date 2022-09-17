using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.Configuration
{
    public interface IEpsilonManager 
    {
        float GetEpsilon(string dataName, string unitName, float wantedEpsilon = 0);
        float GetEpsilon(string dataName, string unitName, bool forGauge = false, float wantedEpsilon = 0);
    }
    public delegate void DataDefinitionCreatedDelegate(IDataDefinition definition, IDataDefinition monitorableDef);
    public interface IExportableDataDefinitionProperties
    {

    }
    public interface IDataDefinitionProperties : IBrowsableItem
    {
        string Access { get; set; }
        string Category { get; set; }
        double CorrectionFactor { get; set; }
        string Information { get;  }
        float Epsilon { get; set; }
        string AlternateID { get; set; }
        string PrimaryKey { get; set; }
        bool IsDeprecated { get; set; }
        bool IsReadOnly { get; }
        bool IsMonitored { get; }
        string Key { get; set; }
        string LinkedEntry { get; set; }
        string Name { get; set; }
        string OffsetMode { get; set; }
        string ProviderName { get; set; }
        bool Selectable { get; set; }
        int Size { get; set; }
        string SubCategory { get; set; }
        string TypeName { get; }
        string UnitsName { get; set; }
        
        string ValueType { get; set; }
        string WriteMode { get; set; }
        string WriteParameters { get; set; }
        int DefinitionKey { get; }
        bool ExcludeKeyFromSearch { get; set; }
    }

    public interface IDataDefinition : IIsMonitorable, IDataDefinitionProperties, IExpandable<IDataDefinition>, ICustomCloneable<IDataDefinition>, IObjectWithOptions
    {
        string AlternateNormalizer { get; set; }
        string CustomNormalizer { get; set; }
        string AvailableDataProviders { get; }
        string DefaultNormalizer { get; set; }
        string DefaultValue { get; set; }
        string DisplayName { get; set; }
        string GlobalName { get; set; }
        bool Disposable { get; set; }
        bool IsValid { get; }

        bool DoesSupportControls { get; }
        bool IsSimConnectData { get; }

        bool HasCustomPrimaryKey { get; }
        IValueNormalizer Normalizer { get; }
        SPADDefinitionTypes DefinitionType { get; set; }
        IValueProvider ValueProvider { get; set; }
        IDataProvider DataProvider { get; }
        IDataDefinition LinkedDataDefinition { get; }
        IValueRange Range { get; }

        ushort DataIndex { get; set; }
        bool IsProviderDataDefinition { get; }

        object UserData { get; set; }
        T GetUserData<T>();

        IEnumerable<string> GetIDs();
        bool HasAlternateUnits { get; }
        List<string> AlternateUnits { get; }
        string PrimaryID { get; }

        object GetRawValue();
        double GetValue();
        void SetValue(double val);
        string GetValueString(string displayFormat);

        decimal CheckRange(decimal val);
        void FixUp();
        void ProcessOutgoing(IValueConnector connection, object data);
        object ConvertValue(object val);

        bool IsString { get; }
        
    }

    public interface IValueRange
    {
        decimal Minimum { get; }
        decimal Maximum { get; }
        decimal Step { get; }
        bool HasRange { get; }
        bool RollOver { get; }

        decimal CheckRange(decimal val);
        bool Parse(string strVal);
    }

    public interface IDataDefinitions
    {
        SPADDefinitionTypes DefinitionType { get; }
        IReadOnlyList<IDataDefinition> Definitions { get; }

        IDataDefinition FindByKey(string key);
        void Add(IDataDefinition toAdd);
        void Save(string filename);
    }

    public interface IIsMonitorable
    {
        IMonitorableValue Monitorable { get; }
        IMonitorableValue NoCreateMonitorable { get; }
        bool CanMonitor { get; }
    }

    public interface IBrowsableItem
    {
        string BrowserValueString { get; }
        string ID { get; }
        string DisplayString { get; set; }

        string SortID { get; }
        HashSet<string> AdditionalSortIDs { get; }
        string Usage { get; set; }
        string SearchKey { get; }

        
    }
    public interface IExpandable<T>
    {
        int ChildCount { get; set; }
        IEnumerable<T> Expand();
    }

    public interface IControlDefinition
    {
        int NumberOfParameters { get; set; }
    }
}
