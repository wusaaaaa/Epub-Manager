using Caliburn.Micro;
using DevExpress.Mvvm;
using Epub_Manager.Extensions;
using System.IO;

namespace Epub_Manager.Views.EpubData.Rename
{
    public class RenameViewModel : Screen
    {
        private FileInfo _oldFile;
        private string _newName;
        private string _oldName;

        public string NewName
        {
            get { return this._newName; }
            set { this.SetProperty(ref this._newName, value); }
        }

        public string OldName
        {
            get { return this._oldName; }
            set { this.SetProperty(ref this._oldName, value); }
        }

        public DelegateCommand Save { get; }
        public DelegateCommand Cancel { get; }

        public RenameViewModel()
        {
            this.DisplayName = "Rename file";

            this.Save = new DelegateCommand(this.SaveImpl, this.CanSaveImpl);
            this.Cancel = new DelegateCommand(this.CancelImpl);
        }

        public void Initialize(FileInfo file)
        {
            this._oldFile = file;
            this.OldName = this._oldFile.Name;
        }

        private bool CanSaveImpl()
        {
            if (this.OldName != this._newName)
                return true;

            return false;
        }

        private void SaveImpl()
        {
            this.TryClose(true);
        }

        private void CancelImpl()
        {
            this.TryClose(false);
        }
    }
}