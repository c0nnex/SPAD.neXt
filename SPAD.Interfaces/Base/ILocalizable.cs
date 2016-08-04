using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface ILocalizable
    {
        String LocalizedString(String resourceKey, IReadOnlyDictionary<string, string> vars);
        String LocalizedString(String resourceKey);
        Object LocalizedResource(String resourceKey);
        bool HasLocalizedString(string resourceKey);
    }
}
