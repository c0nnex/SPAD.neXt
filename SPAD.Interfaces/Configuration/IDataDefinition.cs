using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.Configuration
{
    public interface IExportableDataDefinitionProperties
    {

    }
    public interface IDataDefinitionProperties 
    {
        string Access { get; set; }
        string Category { get; set; }
        double CorrectionFactor { get; set; }
        string Information { get;  }
        float Epsilon { get; set; }
        string ID { get;  }
        string AlternateID { get; set; }
        string PrimaryKey { get; set; }
        bool IsReadOnly { get; }
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
        string Usage { get; set; }
        string ValueType { get; set; }
        string WriteMode { get; set; }
        string WriteParameters { get; set; }
        int DefinitionKey { get; }
        bool ExcludeKeyFromSearch { get; set; }
    }

    public interface IDataDefinition : IIsMonitorable, IDataDefinitionProperties, IExpandable<IDataDefinition>, ICustomCloneable<IDataDefinition>
    {
        string AlternateNormalizer { get; set; }
        string CustomNormalizer { get; set; }
        string AvailableDataProviders { get; }
        string DefaultNormalizer { get; set; }
        string DefaultValue { get; set; }
        string DisplayName { get; set; }
        string DisplayString { get; set; }
        string GlobalName { get; set; }
        bool Disposable { get; set; }
        bool IsValid { get; }

        bool DoesSupportControls { get; }
        bool IsSimConnectData { get; }

        bool HasCustomPrimaryKey { get; }
        IValueNormalizer Normalizer { get; }
        string SortID { get; }
        HashSet<string> AdditionalSortIDs { get; }
        SPADDefinitionTypes DefinitionType { get; set; }
        string SearchKey { get; }
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
        bool CanMonitor { get; }
    }

    public interface IExpandable<T>
    {
        int ChildCount { get; set; }
        IEnumerable<T> Expand();
    }

}
