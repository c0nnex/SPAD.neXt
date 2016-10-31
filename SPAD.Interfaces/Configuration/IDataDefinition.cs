using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
namespace SPAD.neXt.Interfaces.Configuration
{
    public interface IDataDefinitionProperties
    {
        string Access { get; }
        string Category { get; }
        double CorrectionFactor { get; }
        string Information { get; }
        float Epsilon { get; }
        string ID { get; }
        string AlternateID { get; }
        string PrimaryKey { get; }
        bool IsReadOnly { get; }
        string Key { get; }
        string LinkedEntry { get; }
        string Name { get; }
        string OffsetMode { get; }
        string ProviderName { get; }
        bool Selectable { get; }
        int Size { get; }
        string SubCategory { get; }
        string TypeName { get; }
        string UnitsName { get; }
        string Usage { get; }
        string ValueType { get; }
        string WriteMode { get; }
        string WriteParameters { get; }
        int DefinitionKey { get; }
        bool ExcludeKeyFromSearch { get; set; }
    }

    public interface IDataDefinition : IIsMonitorable, IDataDefinitionProperties
    {
        string Access { get; set; }
        string AlternateNormalizer { get; set; }
        string CustomNormalizer { get; set; }
        string Category { get; set; }
        bool CheckBoolean(int nVal);

        double CorrectionFactor { get; set; }
        string Information { get; }
        string DefaultNormalizer { get; set; }
        string DefaultValue { get; set; }
        string DisplayName { get; set; }
        string DisplayString { get; }
        string GlobalName { get; set; }
        bool Disposable { get; set; }
        float Epsilon { get; set; }
        Type EventValueType { get; }
        string Expression { get; set; }
        string ID { get; }
        string AlternateID { get; }
        string PrimaryKey { get; }
        bool IsReadOnly { get; }
        string Key { get; set; }
        string LinkedEntry { get; set; }
        double MinimumChange { get; }
        string Name { get; set; }
        IValueNormalizer Normalizer { get; }
        string OffsetMode { get; set; }
        string ProviderName { get; }
        bool Selectable { get; set; }
        int Size { get; set; }
        string SortID { get; }
        string SubCategory { get; set; }
        string TypeName { get; }
        string UnitsName { get; set; }
        string Usage { get; set; }
        string ValueType { get; set; }
        string WriteMode { get; set; }
        string WriteParameters { get; set; }
        SPADDefinitionTypes DefinitionType { get; set; }
        int DefinitionKey { get; }
        string SearchKey { get; }
        IValueProvider ValueProvider { get; }
        IDataDefinition LinkedDataDefinition { get; }

        object Clone();
        bool IsBoolean();
        IEnumerable<string> GetIDs();
        bool HasAlternateUnits { get; }
        List<string> AlternateUnits { get; }
        string PrimaryID { get; }

        double GetValue();
        void SetValue(double val);
        string GetValueString(string displayFormat);

        void FixUp();
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
    }

}
