using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.FSUIPC
{
    public delegate void FSUIPCPropertyChangedEventHandler(object sender, IFSUIPCPropertyChangedEventArgs e);

    public interface IFSUIPCController
    {
        event EventHandler FSUIPCConnected;

        bool FSUIPC_IsConnected { get; }
        IMonitorableValue FSUIPC_AddOffset(string address, string fireEvent);

        object GetValueByOffset(int address);
        
    }
}
