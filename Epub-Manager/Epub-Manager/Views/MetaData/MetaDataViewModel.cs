using Caliburn.Micro;
using Epub_Manager.Views.Shell;

namespace Epub_Manager.Views.MetaData
{
    public class MetaDataViewModel : Screen, IShellItem
    {
        public MetaDataViewModel()
        {
            this.DisplayName = "MetaData";
        }

        public void Save()
        {
        }

        public void Cancel()
        {
        }
    }
}