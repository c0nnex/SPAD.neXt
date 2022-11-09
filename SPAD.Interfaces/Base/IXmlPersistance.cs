using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces
{
    public interface IConditionalSerialize
    {
        bool ShouldSerializeThis(XmlSerilizationPurpose xmlSerilizationPurpose);
    }

    public interface IXmlAnyObject : IConditionalSerialize
    {
        
    }

    public interface IXmlCallBackOptions
    {
        bool EnableCallbacks { get; }
        bool SerializeID { get; }
        bool SerializeTarget { get; }
        XmlSerilizationPurpose XmlSerilizationPurpose { get; }
    }

    public interface IXmlPersistence
    {
        void OnBeforeSaving();
        void OnAfterSaving();
        void OnAfterLoading();
    }

   
}
