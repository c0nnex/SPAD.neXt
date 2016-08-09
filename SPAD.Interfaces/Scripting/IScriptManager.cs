using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Scripting
{
    public interface IScriptManager
    {
        IEnumerable<string> GetAllScriptValueProviders();
        IScriptValueProvider FindScriptValueProvider(string name);
        IEnumerable<string> GetAllScriptActions();
        IScriptAction FindScriptAction(string name);
    }

    public sealed class ScriptManager 
    {
        private static IScriptManager _ScriptManger { get; set; }

        public static void SetHandler(IScriptManager handler)
        {
            _ScriptManger = handler;
        }

        public static IScriptAction FindScriptAction(string name)
        {
            return _ScriptManger.FindScriptAction(name);
        }

        public static IScriptValueProvider FindScriptValueProvider(string name)
        {
            return _ScriptManger.FindScriptValueProvider(name);
        }

        public static IEnumerable<string> GetAllScriptActions()
        {
            return _ScriptManger.GetAllScriptActions();
        }

        public static IEnumerable<string> GetAllScriptValueProviders()
        {
            return _ScriptManger.GetAllScriptValueProviders();
        }
    }
}
