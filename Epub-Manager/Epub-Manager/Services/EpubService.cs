using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Services;
using Epub_Manager.Windsor.Interceptors;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Epub_Manager.Services
{
    public class EpubService : IEpubService
    {
        [CatchException("Error while getting the cover image")]
        public BitmapImage GetCoverImage(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var opf = this.GetOPF(archive);

                var coverFileName = this.GetCoverFileName(opf);
                if (coverFileName == null)
                    throw new EpubException("No cover file exists.");

                var validFileEndings = new[] { ".jpg", ".jpeg" };

                var entry = archive.Entries.FirstOrDefault(f => f.Name.StartsWith(coverFileName) && validFileEndings.Any(d => f.Name.EndsWith(d)));

                if (entry == null)
                    throw new EpubException("Cover not found.");

                using (var entryStream = entry.Open())
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = entryStream;
                    image.EndInit();

                    return image;
                }
            }
        }

        [CatchException("Error while loading the table of content")]
        public TableOfContentEntry GetToC(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var tocEntry = archive.Entries.FirstOrDefault(f => f.Name.EndsWith(".ncx"));
                if (tocEntry == null)
                    throw new EpubException("No table of content found.");
                
                using (var tocStream  = tocEntry.Open())
                using (var streamReader = new StreamReader(tocStream, Encoding.UTF8))
                {
                    var tocXmlDocument = XDocument.Load(streamReader);

                    var docTitle = tocXmlDocument?.Root?.Elements().FirstOrDefault(f => f.Name.LocalName == "docTitle")?.Elements().FirstOrDefault(f => f.Name.LocalName == "text")?.Value;

                    var navMapNode = tocXmlDocument?.Root?.Elements().FirstOrDefault(f => f.Name.LocalName == "navMap");
                    var tocEntries = this.ReadTableOfContent(navMapNode);

                    return new TableOfContentEntry
                    {
                        Name = docTitle,
                        SubEntries = tocEntries
                    };
                }
            }
        }
        
        [CatchException("Error while loading the meta data")]
        public MetaData GetMetaData(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));


            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var doc = this.GetOPF(archive);

                var metata = new MetaData
                {
                    Title = this.TryGetNodeContent(doc, "title"),
                    Identifier = this.TryGetNodeContent(doc, "identifier"),
                    Creator = new Creator
                    {
                        CreatorName = this.TryGetNodeContent(doc, "creator"),
                        FileAs = this.TryGetAttributeContent(doc, "creator", "file-as"),
                        Role = this.TryGetAttributeContent(doc, "creator", "role")
                    },
                    Date = this.TryGetNodeContent(doc, "date", "event", "creation"),
                    Source = this.TryGetNodeContent(doc, "source"),
                    Description = this.TryGetNodeContent(doc, "description"),
                    Format = this.TryGetNodeContent(doc, "format"),
                    Language = this.TryGetNodeContent(doc, "language"),
                    Publisher = this.TryGetNodeContent(doc, "publisher"),
                    Subject = this.TryGetNodeContent(doc, "subject"),
                    Type = this.TryGetNodeContent(doc, "type")

                };

                return metata;



            }

            return null;
        }

        [CatchException("Error while loading the images in the epub")]
        public List<BitmapImage> GetAllImages(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var validFileEndings = new[] {".jpg", ".jpeg"};

                var images = archive.Entries.Where(f => validFileEndings.Any(d => f.Name.EndsWith(d))).ToList();

                var returnList = new List<BitmapImage>();

                foreach (var zipArchiveEntry in images)
                {
                    using (var entryStream = zipArchiveEntry.Open())
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = entryStream;
                        image.EndInit();

                        returnList.Add(image);
                    }
                }

                return returnList;
            }
        }

        #region Private Methods

        private string TryGetNodeContent(XDocument document, string name, string attributeToFilter = null, string expectedValue = null)
        {
            var metadataNode = document?.Root?.Elements().First(f => f.Name.LocalName == "metadata");
            return metadataNode
                ?.Elements()
                .FirstOrDefault(f => f.Name.LocalName == name && (attributeToFilter == null || f.Attributes().Any(d => d.Name.LocalName == attributeToFilter && d.Value == expectedValue)))
                ?.Value;
        }

        private string TryGetAttributeContent(XDocument document, string node, string attribute)
        {
            var metadataNode = document?.Root?.Elements().First(f => f.Name.LocalName == "metadata");
            return metadataNode?.Elements().FirstOrDefault(f => f.Name.LocalName == node)?.Attributes().FirstOrDefault(f => f.Name.LocalName == attribute)?.Value;
        }

        private string GetCoverFileName(XDocument document)
        {
            var metadatanode = document?.Root?.Elements().FirstOrDefault(f => f.Name.LocalName == "metadata");

            return metadatanode
                ?.Elements()
                .FirstOrDefault(f => f.Name.LocalName == "meta" && f.Attributes().Any(d => d.Name.LocalName == "name" && d.Value == "cover"))
                ?.Attributes()
                .FirstOrDefault(f => f.Name.LocalName == "content")
                ?.Value;
        }

        private XDocument GetOPF(ZipArchive archive)
        {
            foreach (var zipArchiveEntry in archive.Entries)
            {

                if (zipArchiveEntry.Name.EndsWith(".opf"))
                    using (var entryStream = zipArchiveEntry.Open())
                    using (var streamReader = new StreamReader(entryStream, Encoding.UTF8))
                    {
                        return XDocument.Parse(streamReader.ReadToEnd());
                    }
            }
            throw new EpubException("No content found.");
        }

        private List<TableOfContentEntry> ReadTableOfContent(XElement node)
        {
            var result = new List<TableOfContentEntry>();

            foreach (var navPoint in node.Elements().Where(f => f.Name.LocalName == "navPoint"))
            {
                var text = navPoint.Elements().FirstOrDefault(f => f.Name.LocalName == "navLabel")?.Elements().FirstOrDefault(f => f.Name.LocalName == "text")?.Value;
                var subEntries = this.ReadTableOfContent(navPoint);

                result.Add(new TableOfContentEntry
                {
                    Name = text,
                    SubEntries = subEntries
                });
            }

            return result;
        }

        #endregion
    }
}