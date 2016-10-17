using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface IDirectoryModel : IChangeTracking
    {
        string ApplicationDataDirectory { get; set; }
        string ApplicationDocumentsDirectory { get; set; }
        string ApplicationMainDirectory { get; }
        string ConfigurationDirectory { get; }
        string LogDirectory { get; }
        string ProfilesDirectory { get; }
        string GaugesDirectory { get; }
        string ScriptsDirectory { get; }
        string CacheDirectory { get; }

        string ParseName(string nameIn, Dictionary<string, string> additionalValues = null);
        string TokenizeName(string nameIn, Dictionary<string, string> additionalValues = null);

        void RevertChanges();
        void SetupDirectories();
        
        string GetDirectory(APPLICATION_DIRECTORY scope, string subDir = null);

        string GetFilename(APPLICATION_DIRECTORY scope, string filename);
        string GetFilename(APPLICATION_DIRECTORY scope, string subDir, string filename);
    }
}
