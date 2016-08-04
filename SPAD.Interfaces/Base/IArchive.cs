using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Base
{
    public interface IArchive : IDisposable
    {
        string ArchiveName { get; }
        bool IsValid { get; }
        IArchiveEntry GetFileInfo(string filename);
        Stream GetFile(string filename);
        bool FileExists(string filename);
        IReadOnlyList<string> GetFiles(string pattern);
    }

    public interface IArchiveEntry
    {
        long CompressedLength { get; }
        string FullName { get; }
        DateTimeOffset LastWriteTime { get; }
        long Length { get; }
        string Name { get; }
    }
}
