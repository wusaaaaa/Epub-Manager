using System.Collections.Generic;
using Caliburn.Micro;
using DevExpress.Utils;

namespace Epub_Manager.Views.Shell
{
    public class ShellViewModel : Conductor<IShellItem>.Collection.OneActive
    {
        public ShellViewModel(IEnumerable<IShellItem> shellItems)
        {
            Guard.ArgumentNotNull(shellItems, nameof(shellItems));

            this.DisplayName = "Epub Manager";

            this.Items.AddRange(shellItems);

        }
    }
}