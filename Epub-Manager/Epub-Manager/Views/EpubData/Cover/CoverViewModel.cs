using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData.Cover
{
    public class CoverViewModel : Screen, IEpubDetails
    {
        private readonly IEpubService _epubService;
        private readonly IExceptionHandler _exceptionHandler;
        private ImageSource _coverImage;

        public ImageSource CoverImage
        {
            get { return this._coverImage; }
            set { this.SetProperty(ref this._coverImage, value); }
        }

        public CoverViewModel(IEpubService epubService, IExceptionHandler exceptionHandler)
        {
            this._exceptionHandler = exceptionHandler;
            this._epubService = epubService;

            this.DisplayName = "Cover";
        }

        public async Task FileChanged(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            try
            {
                this.CoverImage = null;

                var coverFile = this._epubService.GetCoverImage(file);

                if (coverFile == null)
                    return;

                this.CoverImage = coverFile;
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