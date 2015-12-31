using System.IO;

namespace Epub_Manager.Core.Services
{
    public interface IRenameService
    {
        void RenameFile(FileInfo file, string fileName);
    }
}