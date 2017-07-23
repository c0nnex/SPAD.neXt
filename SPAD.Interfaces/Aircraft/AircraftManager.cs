using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Aircraft
{
    public interface IIndexAircraft
    {
        bool CanIndexAircraft();
        void IndexAircraft(Func<string,bool> progressAction, Action whenDoneAction);
    }
}
