﻿using DevExpress.Utils;
using Epub_Manager.Core;
using Epub_Manager.Core.Entites;
using Epub_Manager.Core.Services;
using Epub_Manager.Windsor.Interceptors;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                var validFileEndings = new[] {".jpg", ".jpeg"};

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

                using (var tocStream = tocEntry.Open())
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
                    Date = this.GetDatetimeFromString(this.TryGetNodeContent(doc, "date", "event", "creation")),
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

        [CatchException("Error while saving the meta data")]
        public void SaveMetaData(FileInfo file, MetaData metadata)
        {
            Guard.ArgumentNotNull(file, nameof(file));
            Guard.ArgumentNotNull(metadata, nameof(metadata));

            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.ReadWrite))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var doc = this.GetOPF(archive);

                this.UpdateNodeContent(doc, "title", metadata.Title);
                this.UpdateNodeContent(doc, "identifier", metadata.Identifier);
                this.UpdateNodeContent(doc, "creator", metadata.Creator.CreatorName, new Dictionary<string, string>
                {
                    ["file-as"] = metadata.Creator.FileAs,
                    ["role"] = metadata.Creator.Role
                } );
                this.UpdateNodeContent(doc, "date", metadata.Date?.ToString("O"), new Dictionary<string, string>
                {
                    ["event"] = "creation"
                });
                this.UpdateNodeContent(doc, "source", metadata.Source);
                this.UpdateNodeContent(doc, "description", metadata.Description);
                this.UpdateNodeContent(doc, "format", metadata.Format);
                this.UpdateNodeContent(doc, "language", metadata.Language);
                this.UpdateNodeContent(doc, "publisher", metadata.Publisher);
                this.UpdateNodeContent(doc, "subject", metadata.Subject);
                this.UpdateNodeContent(doc, "type", metadata.Type);

                this.WriteOPF(doc, archive);
            }
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

        private void UpdateNodeContent(XDocument document, string name, string value, Dictionary<string, string> attributes = null)
        {
            var metadataNode = document?.Root?.Elements().First(f => f.Name.LocalName == "metadata");

            if (metadataNode == null)
                throw new EpubException("No meta data node in opf file found");

            var node = metadataNode.Elements().FirstOrDefault(f => f.Name.LocalName == name);

            if (node == null)
            {
                node = new XElement(XName.Get(name, "http://purl.org/dc/elements/1.1/"));
                metadataNode.Add(node);
            }

            node.Value = value ?? string.Empty;

            foreach (var attribute in attributes ?? new Dictionary<string, string>())
            {
                var attributeNode = node.Attributes().FirstOrDefault(f => f.Name.LocalName == attribute.Key);
                if (attributeNode == null)
                {
                    attributeNode = new XAttribute(XName.Get(attribute.Key, "http://www.idpf.org/2007/opf"), attribute.Value);
                    node.Add(attributeNode);
                }

                attributeNode.Value = attribute.Value ?? string.Empty;
            }
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

        private void WriteOPF(XDocument document, ZipArchive archive)
        {
            var opf = archive.Entries.FirstOrDefault(f => f.Name.EndsWith(".opf"));

            if(opf == null)
                throw new EpubException("No opf file found");

            var fullOpfFilePath = opf.FullName;
            opf.Delete();

            opf = archive.CreateEntry(fullOpfFilePath);
            using(var stream = opf.Open())
                document.Save(stream);
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

        private DateTime? GetDatetimeFromString(string date)
        {
            var formats = new string[]
            {
                "yyyy",
                "yyyy-MM",
                "yyyy-MM-dd",
            };

            DateTime result;
            if (DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return result;
            }

            return null;
        }



        #endregion
    }
}