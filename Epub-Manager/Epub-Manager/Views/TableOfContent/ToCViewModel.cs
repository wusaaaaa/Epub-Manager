using Caliburn.Micro;
using Epub_Manager.Views.Shell;

namespace Epub_Manager.Views.TableOfContent
{
    public class ToCViewModel : Screen, IShellItem
    {
        public ToCViewModel()
        {
            this.DisplayName = "Table of Content";
        }
    }
}