using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using Epub_Manager.Views.EpubData.Tree;
using Epub_Manager.Views.Shell;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Screen, IShellItem
    {

        #region Fields

        private readonly IEpubService _epubService;
        private readonly IMessageManager _messageManager;
        private BindableCollection<TreeItemViewModel> _treeItems;
        private ImageSource _coverImage;
        private FileInfo _file;

        #endregion

        #region Properties

        public string FolderPath { get; set; }

        public BindableCollection<TreeItemViewModel> TreeItems
        {
            get { return this._treeItems; }
            set { this.SetProperty(ref this._treeItems, value); }
        }

        public ImageSource CoverImage
        {
            get { return this._coverImage; }
            set { this.SetProperty(ref this._coverImage, value); }
        }

        public FileInfo File
        {
            get { return this._file; }
            set { this.SetProperty(ref this._file, value); }
        }

        #endregion

        #region Ctor

        public EpubDataViewModel(IEpubService epubService, IMessageManager messageManager)
        {
            Guard.ArgumentNotNull(epubService, nameof(epubService));
            Guard.ArgumentNotNull(messageManager, nameof(messageManager));

            this._epubService = epubService;
            this._messageManager = messageManager;
            this.DisplayName = "Epub Data";

            this.TreeItems = new BindableCollection<TreeItemViewModel>();
        }

        #endregion

        #region Overrides

        protected override void OnActivate()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory + @"\Settings.ini";

            if (!System.IO.File.Exists(location))
                return;

            using (var reader = new StreamReader(location))
                this.FolderPath = reader.ReadLineAsync().Result;

            this.BuildTree();

            //this.GetCover(this.FileInfo);
        }

        protected override void OnDeactivate(bool close)
        {
            var dialogResult = this._messageManager.Show("Save Changes?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                this.Save();
                close = true;
            }

            else if (dialogResult == MessageBoxResult.Cancel)
                close = false;

            close = true;
            this._epubService.RemoveTempFile(this.File);
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Tree

        private void BuildTree()
        {
            this.TreeItems.Clear();

            var directory = new DirectoryInfo(this.FolderPath);

            var viewModel = IoC.Get<FolderTreeItemViewModel>();
            viewModel.Initialize(directory);
            viewModel.IsExpanded = true;

            this.TreeItems.Add(viewModel);


            this.PopulateChildren(directory, viewModel);
        }

        private void PopulateChildren(DirectoryInfo directoryInfo, TreeItemViewModel parent)
        {
            var directories = directoryInfo.GetDirectories();

            foreach (var directory in directories)
            {
                var viewModel = IoC.Get<FolderTreeItemViewModel>();
                viewModel.Initialize(directory);

                parent.Children.Add(viewModel);

                this.PopulateChildren(directory, viewModel);

                foreach (var fileInfo in directory.GetFiles())
                {
                    var childViewModel = IoC.Get<FileTreeItemViewModel>();
                    childViewModel.Initialize(fileInfo);

                    viewModel.Children.Add(childViewModel);
                }
            }

        }

        #endregion

        #region Cover

        private void GetCover(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            if (file.DirectoryName != null)
            {
                var coverFile = this._epubService.GetCoverImage(new DirectoryInfo(file.DirectoryName));

                this.CoverImage = new BitmapImage(new Uri(coverFile.FullName, UriKind.Relative));
            }
        }

        #endregion
    }
}