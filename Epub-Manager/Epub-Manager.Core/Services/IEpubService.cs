using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Epub_Manager.Core.Services
{
    public interface IEpubService
    {
        BitmapImage GetCoverImage(FileInfo file);

        TableOfContentEntry GetToC(FileInfo file);

        MetaData GetMetaData(FileInfo file);

        List<BitmapImage> GetAllImages(FileInfo file);
    }
}