using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Entites;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace Epub_Manager.Views.EpubData.TableofContent
{
    public class TableOfContentViewModel : Screen, IEpubDetails
    {
        private readonly IEpubService _epubService;
        private readonly IExceptionHandler _exceptionHandler;
        private BindableCollection<TableOfContentEntry> _toC;

        public BindableCollection<TableOfContentEntry> ToC
        {
            get { return this._toC; }
            set { this.SetProperty(ref this._toC, value); }
        }

        public TableOfContentViewModel(IEpubService epubService, IExceptionHandler exceptionHandler)
        {
            this._epubService = epubService;
            this._exceptionHandler = exceptionHandler;

            this.ToC = new BindableCollection<TableOfContentEntry>();

            this.DisplayName = "Table of Content";
        }

        public async Task FileChanged(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            try
            {
                this.ToC.Clear();

                var toc = this._epubService.GetToC(file);

                if (toc == null)
                    return;

                this.ToC.Add(toc);
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }
        public bool CanSave()
        {
            return false;
        }

        public Task Save()
        {
            return Task.CompletedTask;
        }

        public Task CancelChanges()
        {
            return Task.CompletedTask;
        }
    }
}