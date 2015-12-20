using Caliburn.Micro;
using Epub_Manager.Images;
using System.IO;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData.Tree
{
    public class FolderTreeItemViewModel : TreeItemViewModel
    {

        public DirectoryInfo Item { get; set; }

        public override string DisplayText => this.Item.Name;

        public override ImageSource Image => Resources.Folder;

        public FolderTreeItemViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        public void Initialize(DirectoryInfo directoryInfo)
        {
            this.Item = directoryInfo;
        }

    }
}