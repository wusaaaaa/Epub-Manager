using System.IO;

namespace Epub_Manager.Core.Services
{
    public interface IEpubService
    {
        DirectoryInfo UnzipToTemporaryFile(FileInfo file);

        FileInfo GetCoverImage(DirectoryInfo file);

        FileInfo GetToC(DirectoryInfo file);

        void RemoveTempFile(FileInfo file);

    }
}