using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
