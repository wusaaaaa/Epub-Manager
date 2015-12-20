using Caliburn.Micro;
using Epub_Manager.Views.Shell;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Screen, IShellItem
    {
        public EpubDataViewModel()
        {
            this.DisplayName = "Epub Data";
        }
    }
}