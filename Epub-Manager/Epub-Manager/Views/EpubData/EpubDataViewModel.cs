using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using Epub_Manager.Views.EpubData.Tree;
using Epub_Manager.Views.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Screen, IShellItem, IHandle<TreeItemSelected>
    {
        #region Fields

        private readonly IEpubService _epubService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExceptionHandler _exceptionHandler;
        private BindableCollection<TreeItemViewModel> _treeItems;
        private ImageSource _coverImage;
        private FileInfo _file;
        private BindableCollection<TableOfContentEntry> _toC;
        private MetaData _metaData;
        private BindableCollection<ImageSource> _images;

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

        public BindableCollection<TableOfContentEntry> ToC
        {
            get { return this._toC; }
            set { this.SetProperty(ref this._toC, value); }
        }

        public MetaData MetaData
        {
            get { return this._metaData; }
            set { this.SetProperty(ref this._metaData, value); }
        }

        public BindableCollection<ImageSource> Images
        {
            get { return this._images; }
            set { this.SetProperty(ref this._images, value); }
        }

        #endregion

        #region Ctor

        public EpubDataViewModel(IEpubService epubService, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler)
        {
            Guard.ArgumentNotNull(epubService, nameof(epubService));
            Guard.ArgumentNotNull(eventAggregator, nameof(eventAggregator));
            Guard.ArgumentNotNull(exceptionHandler, nameof(exceptionHandler));

            this._epubService = epubService;
            this._eventAggregator = eventAggregator;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Epub Data";

            this._eventAggregator.Subscribe(this);
            this.TreeItems = new BindableCollection<TreeItemViewModel>();
            this.ToC = new BindableCollection<TableOfContentEntry>();
            this.Images = new BindableCollection<ImageSource>();
        }

        #endregion

        #region Lifecycle

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

        #region Methods
        private void GetCover(FileInfo info)
        {
            Guard.ArgumentNotNull(info, nameof(info));

            try
            {
                this.CoverImage = null;

                var coverFile = this._epubService.GetCoverImage(info);

                if (coverFile == null)
                    return;

                this.CoverImage = coverFile;
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }

        private void GetMetaData(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            try
            {
                this.MetaData = null;

                var result = this._epubService.GetMetaData(file);

                if (result == null)
                    return;

                this.MetaData = result;
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }

        private void GetToC(FileInfo info)
        {
            Guard.ArgumentNotNull(info, nameof(info));

            try
            {
                this.ToC.Clear();

                var toc = this._epubService.GetToC(info);

                if (toc == null)
                    return;

                this.ToC.Add(toc);
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }

        private void GetImages(FileInfo info)
        {
            Guard.ArgumentNotNull(info, nameof(info));

            try
            {
                this.Images.Clear();

                var images = this._epubService.GetAllImages(this.File);

                if (images == null)
                    return;

                this.Images.AddRange(images);
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }

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
            this.GetImages(this.File);
        }

        #endregion

    }
}