using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Profile
{
    public interface IProfile
    {
        bool IsDirty { get; }
        string Name { get; }
        string Filename { get; }

        void SetDirty(bool isDirty = true);
        IProfileOption GetOption(string key);
        void DeleteOption(string key);

        IReadOnlyList<IDeviceProfile> Devices { get; }

        void Save(bool IncrementVersion = true);
        void MarkForAutoSave();
    }
}
