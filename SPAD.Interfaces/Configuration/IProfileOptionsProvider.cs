using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SPAD.neXt.Interfaces.Configuration
{
    public interface ISettingsProvider : IProfileOptionsProvider, IWindowPlacementProvider
    { }

    public interface IProfileOptionsProvider
    {
        IProfileOption AddOption(string key, Interfaces.Profile.ProfileOptionTypes type, string defaultValue, bool needrestart = false, bool editable = false, bool hidden = false,string groupName="Other");
        IProfileOption GetOption(string key);
        T GetOption<T>(string key);
        void RemoveOption(string key);
        List<IProfileOption> GetOptions(string startWith);
        bool HasOption(string key);
        void SetOption(string key, string value);
    }

    public interface IWindowPlacementProvider
    {
        IWindowPlacement GetWindowPlacement(string key);
        void SetWindowPlacement(IWindowPlacement placement);
    }

    public interface IWindowPlacement
    {
        string Key { get; }

        double Top { get; set; }
        double Left { get; set; }
        double Height { get; set; }
        double Width { get; set; }

        bool HasValues { get; }
        void ApplyPlacement(Window w, bool force = false);
        bool SavePlacement(Window w);

        T GetOption<T>(string key, T defaultValue = default(T));
        void SetOption(string key, object value);
    }
}
