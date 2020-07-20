using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces
{

    public interface IXmlAnyObject
    {
        bool ShouldSerializeThis();
    }

    public interface IXmlPersistence
    {
        void OnBeforeSaving();
        void OnAfterSaving();
        void OnAfterLoading();
    }

   
}
