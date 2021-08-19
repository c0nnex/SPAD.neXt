using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.SimConnect
{
    public interface ISimConnectDynamicObject
    {
        uint ObjectID { get; }
        uint DefineID { get; }
        uint RequestID { get; }
        string TypeName { get; }
        bool IsClientStructure { get; }
        int CountDataItems { get; }

        IReadOnlyDictionary<string, string> Dump();
        bool IsKnown(string offset);
        object GetValue(string index);
        void SetValue(string index, object value);
        bool IsDirty(short idx);

        event SPADEventHandler PropertyChanged;
        event EventHandler<ISimConnectDynamicObject,List<string>> DataChanged;
        void OnDataChanged();
        void RaiseChangedEvent(ISPADEventArgs e);
        void Reset(int idx = -1);
        void SetClean(short idx);
        bool SetDirty(short idx);

        IDynamicSimConnectDataItem AddDataItem(string id, string name, string units, float epsilon = 0, double correctionfactor = 1.0);
        IReadOnlyList<string> GetAllKeys();
        IDynamicSimConnectDataItem Find(string name);

        void BeginUpdate();
        void EndUpdate();
        void SetSingleUpdateMode(bool UseSingleUpdateMode);
        bool IsUpdating();
    }

    public interface IDynamicSimConnectDataItem : IDataItemAttributeBase, IDataItemBoundBase
    {
        double CorrectionFactor { get; set; }
        object Value { get; }
        object OldValue { get; }
        Type FieldType { get; }
        object GetVal(object obj);
        short InternalIndex { get; set; }
        void OnPropertyChanged(object oldValue, object newValue);
        event SPADEventHandler PropertyChanged;
        void SetVal(object obj, object val);
        ISimConnectDynamicObject Parent { get; }
        string ToString();
        void UpdateValue(object val, bool markDirty = true);
        void ForceUpdate();
    }
}
