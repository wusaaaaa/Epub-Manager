using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using Epub_Manager.Views.EpubData.Tree;
using Epub_Manager.Views.Shell;
using System;
using System.IO;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Screen, IShellItem, IHandle<TreeItemSelected>
    {
        #region Fields

        private readonly IEpubService _epubService;
        private readonly IMessageManager _messageManager;
        private readonly IEventAggregator _eventAggregator;
        private BindableCollection<TreeItemViewModel> _treeItems;
        private ImageSource _coverImage;
        private FileInfo _file;
        private DirectoryInfo _tempFile;
        private BindableCollection<string> _toC;
        private MetaDataViewModel _metaData;

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
            set
            {
                if (this.SetProperty(ref this._file, value))
                    this.FileChanged();
            }
        }

        public BindableCollection<string> ToC
        {
            get { return this._toC; }
            set { this.SetProperty(ref this._toC, value); }
        }

        public MetaDataViewModel MetaData
        {
            get { return this._metaData; }
            set { this.SetProperty(ref this._metaData, value); }
        }

        #endregion

        #region Ctor

        public EpubDataViewModel(IEpubService epubService, IMessageManager messageManager, IEventAggregator eventAggregator)
        {
            Guard.ArgumentNotNull(epubService, nameof(epubService));
            Guard.ArgumentNotNull(messageManager, nameof(messageManager));

            this._epubService = epubService;
            this._messageManager = messageManager;
            this._eventAggregator = eventAggregator;
            this.DisplayName = "Epub Data";

            this._eventAggregator.Subscribe(this);
            this.TreeItems = new BindableCollection<TreeItemViewModel>();
            this.ToC = new BindableCollection<string>();
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

            }
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                var childViewModel = IoC.Get<FileTreeItemViewModel>();
                childViewModel.Initialize(fileInfo);

                parent.Children.Add(childViewModel);
            }

        }

        #endregion

        #region Cover

        private void GetCover(FileInfo info)
        {
            Guard.ArgumentNotNull(info, nameof(info));

            this.CoverImage = null;

            var coverFile = this._epubService.GetCoverImage(info);

            if (coverFile == null)
                return;

            this.CoverImage = coverFile;
        }

        #endregion

        #region MetaData

        private void GetMetaData(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            this.MetaData = null;

            var result = this._epubService.GetMetaData(file);

            if (result == null)
                return;

            this.MetaData = result;
        }

        #endregion


        #region ToC

        private void GetToC(FileInfo info)
        {
            Guard.ArgumentNotNull(info, nameof(info));

            this.ToC.Clear();

            var toc = this._epubService.GetToC(info);

            if (toc == null)
                return;

            this.ToC.AddRange(toc);
        }

        #endregion

        #region Methods

        public void Handle(TreeItemSelected message)
        {
            if (message.TreeItem is FileTreeItemViewModel)
            {
                var fileTree = (FileTreeItemViewModel)message.TreeItem;

                this.File = fileTree.File;
            }
        }

        private void FileChanged()
        {
            this.GetCover(this.File);
            this.GetToC(this.File);
            this.GetMetaData(this.File);
        }

        #endregion

    }
}