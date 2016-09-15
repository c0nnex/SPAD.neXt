using SPAD.neXt.Interfaces.Aircraft.CDU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Aircraft
{
    public interface IAircraft
    {
        string Name { get; }

        ICDUScreen GetCDU(CDU_NUMBER cduNumber);
    }
}
