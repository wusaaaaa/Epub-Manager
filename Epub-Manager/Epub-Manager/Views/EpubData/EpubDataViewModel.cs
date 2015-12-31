using Caliburn.Micro;
using DevExpress.Mvvm;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using Epub_Manager.Views.EpubData.Rename;
using Epub_Manager.Views.EpubData.Tree;
using Epub_Manager.Views.Shell;
using Epub_Manager.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Epub_Manager.Views.EpubData
{
    public class EpubDataViewModel : Conductor<IEpubDetails>.Collection.OneActive, IShellItem, IHandle<TreeItemSelected>
    {
        #region Fields

        private readonly IExceptionHandler _exceptionHandler;
        private readonly IRenameService _renameService;
        private readonly IWindowManager _windowManager;

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

        public AsyncCommand Save { get; }
        public AsyncCommand CancelChanges { get; }
        public AsyncCommand Rename { get; }

        #endregion

        #region Ctor

        public EpubDataViewModel(IEnumerable<IEpubDetails> details, IExceptionHandler exceptionHandler, IRenameService renameService, IWindowManager windowManager)
        {
            Guard.ArgumentNotNull(exceptionHandler, nameof(exceptionHandler));
            Guard.ArgumentNotNull(renameService, nameof(renameService));
            Guard.ArgumentNotNull(windowManager, nameof(windowManager));

            this._exceptionHandler = exceptionHandler;
            this._renameService = renameService;
            this._windowManager = windowManager;

            this.DisplayName = "Epub Data";

            this.TreeItems = new BindableCollection<TreeItemViewModel>();

            this.Items.AddRange(details);
            this.ActiveItem = this.Items.First();

            this.Save = new AsyncCommand(this.SaveImpl, this.CanSaveImpl);
            this.CancelChanges = new AsyncCommand(this.CancelChangesImpl);
            this.Rename = new AsyncCommand(this.RenameImpl, this.CanRenameImpl);
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

        private async void FileChanged()
        {
            foreach (var item in this.Items)
            {
                await item.FileChanged(this.File);
            }
        }

        private bool CanSaveImpl()
        {
            if (this.ActiveItem == null)
                return false;

            return this.ActiveItem.CanSave();
        }

        private async Task SaveImpl()
        {
            try
            {
                await this.ActiveItem.Save();

                this.FileChanged();
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }

        private Task CancelChangesImpl()
        {
            return this.ActiveItem.CancelChanges();
        }

        private bool CanRenameImpl()
        {
            if (this.File != null)
                return true;

            return false;
        }

        private Task RenameImpl()
        {
            try
            {
                var viewModel = IoC.Get<RenameViewModel>();
                viewModel.Initialize(this.File);

                var result = this._windowManager.ShowDialog(viewModel, null, WindowSettings.With().AutoSize().NoIcon());

                if (result == true)
                {
                    this._renameService.RenameFile(this.File, viewModel.NewName);

                    this.OnActivate();
                }

                return Task.CompletedTask;
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
                return Task.CompletedTask;
            }
        }

        #endregion
    }
}