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
        bool ShouldSerializeThis();
    }
    public interface IXmlAnyObject : IConditionalSerialize
    {
        
    }

    public interface IXmlPersistence
    {
        void OnBeforeSaving();
        void OnAfterSaving();
        void OnAfterLoading();
    }

   
}
