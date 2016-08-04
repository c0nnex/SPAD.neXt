using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Scripting
{
    public interface IScript
    {
        
    }


    public interface IScriptValueProvider : IScript
    {
        double ProvideValue(IApplication app);
    }

    public interface IScriptActivation
    {
        void Activate();
        void Deactivate();
    }

    public interface IScriptCreation
    {
        void Initialize(IApplication app);
        void Deinitialize();
    }
}
