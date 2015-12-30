using System.IO;

namespace Epub_Manager.Views.EpubData
{
    public interface IEpubDetails
    {
        void FileChanged(FileInfo file);
    }
}