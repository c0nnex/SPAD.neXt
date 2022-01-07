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
        event EventHandler<ICDUScreen, EventArgs> CDUChanged;
        event EventHandler<CDU_NUMBER, CDU_LED, int> LEDChanged;
        string Name { get; }

        void Reset();
        ICDUScreen GetCDU(CDU_NUMBER cduNumber);
        void SetCDU(CDU_NUMBER cduNumber, GenericCDUScreen cduData);
        void UpdateLedStatus(CDU_NUMBER cduNumber, CDU_LED led, int isOn);
    }


}
