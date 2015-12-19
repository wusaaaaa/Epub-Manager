using Caliburn.Micro;
using DevExpress.Mvvm;
using Epub_Manager.Extensions;
using Epub_Manager.Views.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Input;

namespace Epub_Manager.Views.Settings
{
    public class SettingsViewModel : Screen, IShellItem
    {
        private string _folderPath;

        public string FolderPath
        {
            get { return this._folderPath; }
            set { this.SetProperty(ref this._folderPath, value); }
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenFolderBrowserCommand { get; }

        public SettingsViewModel()
        {
            this.DisplayName = "Settings";

            this.SaveCommand = new DelegateCommand(this.Save);
            this.OpenFolderBrowserCommand = new DelegateCommand(this.OpenFolderBrowser);
        }


        protected override void OnActivate()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory + @"\Settings.ini";

            if (!File.Exists(location))
                return;

            using (var reader = new StreamReader(location))
                this.FolderPath = reader.ReadLineAsync().Result;
        }

        private void OpenFolderBrowser()
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            var result = folderDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
                this.FolderPath = folderDialog.FileName;
        }

        private void Save()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory + @"\Settings.ini";


            using (var file = new StreamWriter(location))
                file.WriteLine(this.FolderPath);
        }
    }
}