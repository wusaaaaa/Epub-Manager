using Caliburn.Micro;
using Epub_Manager.Images;
using System.IO;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData.Tree
{
    public class FileTreeItemViewModel : TreeItemViewModel
    {
        public FileInfo File { get; set; }

        public override string DisplayText => this.File.Name;

        public override ImageSource Image => Resources.File;

        public FileTreeItemViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        public void Initialize(FileInfo fileInfo)
        {
            this.File = fileInfo;
        }
    }
}