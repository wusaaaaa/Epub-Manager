using Caliburn.Micro;
using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Extensions;
using System.IO;

namespace Epub_Manager.Views.EpubData.MetaData
{
    public class MetaDataViewModel : Screen, IEpubDetails
    {
        #region Fields

        private readonly IEpubService _epubService;
        private readonly IExceptionHandler _exceptionHandler;
        private Core.MetaData _metaData;
        private string _title;
        private string _creatorName;
        private string _fileAs;
        private string _role;
        private string _description;
        private string _publisher;
        private string _date;
        private string _subject;
        private string _type;
        private string _format;
        private string _identifier;
        private string _source;
        private string _language;

        #endregion

        #region Properties

        public Core.MetaData MetaData
        {
            get { return this._metaData; }
            set
            {
                if (this.SetProperty(ref this._metaData, value))
                {
                    this.Title = this.MetaData?.Title;
                    this.CreatorName = this.MetaData?.Creator?.CreatorName;
                    this.FileAs = this.MetaData?.Creator?.FileAs;
                    this.Role = this.MetaData?.Creator?.Role;
                    this.Description = this.MetaData?.Description;
                    this.Publisher = this.MetaData?.Publisher;
                    this.Date = this.MetaData?.Date;
                    this.Subject = this.MetaData?.Subject;
                    this.Type = this.MetaData?.Type;
                    this.Format = this.MetaData?.Format;
                    this.Identifier = this.MetaData?.Identifier;
                    this.Source = this.MetaData?.Source;
                    this.Language = this.MetaData?.Language;
                }
            }
        }

        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        public string CreatorName
        {
            get { return this._creatorName; }
            set { this.SetProperty(ref this._creatorName, value); }
        }

        public string FileAs
        {
            get { return this._fileAs; }
            set { this.SetProperty(ref this._fileAs, value); }
        }

        public string Role
        {
            get { return this._role; }
            set { this.SetProperty(ref this._role, value); }
        }

        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        public string Publisher
        {
            get { return this._publisher; }
            set { this.SetProperty(ref this._publisher, value); }
        }

        public string Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        public string Subject
        {
            get { return this._subject; }
            set { this.SetProperty(ref this._subject, value); }
        }

        public string Type
        {
            get { return this._type; }
            set { this.SetProperty(ref this._type, value); }
        }

        public string Format
        {
            get { return this._format; }
            set { this.SetProperty(ref this._format, value); }
        }

        public string Identifier
        {
            get { return this._identifier; }
            set { this.SetProperty(ref this._identifier, value); }
        }

        public string Source
        {
            get { return this._source; }
            set { this.SetProperty(ref this._source, value); }
        }

        public string Language
        {
            get { return this._language; }
            set { this.SetProperty(ref this._language, value); }
        }

        #endregion

        #region Ctor

        public MetaDataViewModel(IEpubService epubService, IExceptionHandler exceptionHandler)
        {
            this._epubService = epubService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Meta data";
        }

        #endregion

        #region Methods

        public void FileChanged(FileInfo file)
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

        #endregion

    }
}