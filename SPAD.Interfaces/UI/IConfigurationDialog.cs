using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SPAD.neXt.Interfaces.UI
{
    [Obsolete]
    public interface IConfigurationDialog
    {
        void SetTitle(string ressourceName);
        void SetConfigurationEventHandler(EventHandler handler);
    }

    public interface IOrderableItem
    {
        string Name { get; }
        Guid ID { get; }
    }

    public interface IUIProxy
    { }

    public interface IDialogCallbackProvider
    {
        void CloseDialog(MessageBoxResult result, bool executeCallbacks = true);

    }
}
