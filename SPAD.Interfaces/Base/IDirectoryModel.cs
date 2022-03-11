using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface IDirectoryModel 
    {
        string ApplicationDataDirectory { get; set; }
        string ApplicationDocumentsDirectory { get; set; }
        string ApplicationMainDirectory { get; }
        string ConfigurationDirectory { get; set; }
        string LogDirectory { get; set; }
        string ProfilesDirectory { get; set; }
        string GaugesDirectory { get; set; }
        string ScriptsDirectory { get; set; }
        string CacheDirectory { get; }
        bool HasError { get; }
        string GetErrorString();
        string ParseName(string nameIn, Dictionary<string, string> additionalValues = null);
        string TokenizeName(string nameIn, Dictionary<string, string> additionalValues = null);

        void AcceptChanges();
        void SetupDirectories();
        
        string GetDirectory(APPLICATION_DIRECTORY scope, string subDir = null);

        string GetFilename(APPLICATION_DIRECTORY scope, string filename);
        string GetFilename(APPLICATION_DIRECTORY scope, string subDir, string filename);
        string GetFilename(APPLICATION_DIRECTORY scope, string subDir1, string subDir2, string filename);
    }
}
