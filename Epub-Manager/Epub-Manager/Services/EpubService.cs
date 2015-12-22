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
using System.Xml;
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
                foreach (var zipArchiveEntry in archive.Entries)
                {
                    if (zipArchiveEntry.Name.Equals("Cover.jpg"))
                        using (var entryStream = zipArchiveEntry.Open())
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

            return null;
        }

        [CatchException("Error while loading the table of content")]
        public List<string> GetToC(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));


            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                foreach (var zipArchiveEntry in archive.Entries)
                {
                    if (zipArchiveEntry.Name.EndsWith(".ncx"))
                        using (var entryStream = zipArchiveEntry.Open())
                        using (var xmlReader = new XmlTextReader(entryStream))
                        {
                            var toc = new List<string>();
                            while (xmlReader.Read())
                            {
                                if (xmlReader.Name != "text")
                                    continue;

                                toc.Add(xmlReader.ReadString());
                            }

                            return toc;
                        }
                }

            }

            return null;
        }

        [CatchException("Error while loading the meta data")]
        public MetaDataViewModel GetMetaData(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));


            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                foreach (var zipArchiveEntry in archive.Entries)
                {
                    if (zipArchiveEntry.Name.EndsWith(".opf"))
                        using (var entryStream = zipArchiveEntry.Open())
                        using (var streamReader = new StreamReader(entryStream, Encoding.UTF8))
                        {
                            var doc = XDocument.Parse(streamReader.ReadToEnd());

                            var metata = new MetaDataViewModel
                            {
                                Title = this.TryGetNodeContent(doc, "title"),
                                Identifier = this.TryGetNodeContent(doc, "identifier"),
                                Creator = new CreatorViewModel
                                {
                                    Creator = this.TryGetNodeContent(doc, "creator"),
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
                }

            }

            return null;
        }

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
    }
}