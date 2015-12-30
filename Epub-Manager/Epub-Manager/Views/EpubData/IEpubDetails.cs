using System.IO;
using System.Threading.Tasks;

namespace Epub_Manager.Views.EpubData
{
    public interface IEpubDetails
    {
        Task FileChanged(FileInfo file);

        bool CanSave();
        Task Save();

        Task CancelChanges();
    }
}