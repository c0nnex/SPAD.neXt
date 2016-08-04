using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.FSUIPC
{
    public interface IFSUIPCPropertyChangedEventArgs
    {
        bool Handled { get; set; }
        object NewValue { get; }
        int Offset { get; }
        object OldValue { get; }
        string PropertyName { get; }
    }
}
