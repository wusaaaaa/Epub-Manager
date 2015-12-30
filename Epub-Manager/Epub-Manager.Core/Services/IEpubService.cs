using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Epub_Manager.Core.Entites;

namespace Epub_Manager.Core.Services
{
    public interface IEpubService
    {
        BitmapImage GetCoverImage(FileInfo file);

        TableOfContentEntry GetToC(FileInfo file);

        MetaData GetMetaData(FileInfo file);

        void SaveMetaData(FileInfo file, MetaData metadata);

        List<BitmapImage> GetAllImages(FileInfo file);
    }
}