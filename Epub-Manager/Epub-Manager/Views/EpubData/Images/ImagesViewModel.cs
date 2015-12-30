using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using System.IO;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData.Images
{
    public class ImagesViewModel : Screen, IEpubDetails
    {
        private readonly IEpubService _epubService;
        private readonly IExceptionHandler _exceptionHandler;
        private BindableCollection<ImageSource> _images;

        public BindableCollection<ImageSource> Images
        {
            get { return this._images; }
            set { this.SetProperty(ref this._images, value); }
        }

        public ImagesViewModel(IEpubService epubService, IExceptionHandler exceptionHandler)
        {
            this._epubService = epubService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Images";

            this.Images = new BindableCollection<ImageSource>();
        }

        public void FileChanged(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            try
            {
                this.Images.Clear();

                var images = this._epubService.GetAllImages(file);

                if (images == null)
                    return;

                this.Images.AddRange(images);
            }
            catch (EpubException ex)
            {
                this._exceptionHandler.Handle(ex);
            }
        }
    }
}