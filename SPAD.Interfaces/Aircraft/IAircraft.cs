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

        ICDUScreen CDU_0 { get; }
        ICDUScreen CDU_1 { get; }
        ICDUScreen CDU_2 { get; }

    }
}
