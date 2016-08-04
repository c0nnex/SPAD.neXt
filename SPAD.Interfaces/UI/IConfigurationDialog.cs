using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.UI
{
    [Obsolete]
    public interface IConfigurationDialog
    {
        void SetTitle(string ressourceName);
        void SetConfigurationEventHandler(EventHandler handler);
    }
}
