using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Profile
{
    public interface IProfile : IDeviceImageResolver
    {
        bool IsDummyProfile { get; }
        bool IsDirty { get; }
        string Name { get; }
        string Filename { get; }

        void SetDirty(bool isDirty = true,string caller="");
        IProfileOption GetOption(string key);
        void DeleteOption(string key);

        IReadOnlyList<IDeviceProfile> Devices { get; }
        IDeviceProfile FindTarget(string targetDeviceID);
        bool Save(bool IncrementVersion = true);
        void MarkForAutoSave();
    }
}
