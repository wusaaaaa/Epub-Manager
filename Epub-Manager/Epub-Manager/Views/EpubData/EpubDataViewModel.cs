using Caliburn.Micro;
using DevExpress.Mvvm;
using Epub_Manager.Extensions;
using Epub_Manager.Views.EpubData.Tree;
using Epub_Manager.Views.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Conductor<IEpubDetails>.Collection.OneActive, IShellItem, IHandle<TreeItemSelected>
    {
        #region Fields
        
        private BindableCollection<TreeItemViewModel> _treeItems;
        private FileInfo _file;

        #endregion

        #region Properties

        public string FolderPath { get; set; }

        public BindableCollection<TreeItemViewModel> TreeItems
        {
            get { return this._treeItems; }
            set { this.SetProperty(ref this._treeItems, value); }
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

        #endregion

        #region Commands

        public DelegateCommand SaveMetadata { get; set; }
        public DelegateCommand CancelMetadata { get; set; }

        #endregion

        #region Ctor

        public EpubDataViewModel(IEnumerable<IEpubDetails> details)
        {
            this.DisplayName = "Epub Data";
            
            this.TreeItems = new BindableCollection<TreeItemViewModel>();
            
            this.Items.AddRange(details);
            this.ActiveItem = this.Items.First();
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
            foreach (var item in this.Items)
            {
                item.FileChanged(this.File);
            }
        }

        #endregion
    }
}