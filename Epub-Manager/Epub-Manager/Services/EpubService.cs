using DevExpress.Utils;
using Epub_Manager.Core.Services;
using Epub_Manager.Windsor.Interceptors;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Epub_Manager.Services
{
    public class EpubService : IEpubService
    {
        [CatchException("Error while opening the Epub")]
        public DirectoryInfo UnzipToTemporaryFile(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            var tempFile = Path.GetTempFileName() + Guid.NewGuid();

            ZipFile.ExtractToDirectory(file.DirectoryName, tempFile);

            return new DirectoryInfo(tempFile);
        }

        [CatchException("Error while getting the cover image")]
        public FileInfo GetCoverImage(DirectoryInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            var oebps = file.GetDirectories("OEBPS");

            var images = oebps?.First()?.GetDirectories("Images");

            var cover = images?.First()?.GetFiles("Cover.jpg");

            return cover?.First();
        }

        [CatchException("Error while loading the table of content")]
        public FileInfo GetToC(DirectoryInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            var oebps = file.GetDirectories("OEBPS");

            var toc = oebps?.First()?.GetFiles("toc.ncx");

            return toc?.First();
        }

        [CatchException("Error while removing the temporary file")]
        public void RemoveTempFile(FileInfo file)
        {
            Guard.ArgumentNotNull(file, nameof(file));

            if (file.DirectoryName != null && File.Exists(file.DirectoryName))
                File.Delete(file.DirectoryName);
        }
    }
}