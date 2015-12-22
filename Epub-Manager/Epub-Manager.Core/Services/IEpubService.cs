using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Epub_Manager.Core.Services
{
    public interface IEpubService
    {
        BitmapImage GetCoverImage(FileInfo file);

        List<string> GetToC(FileInfo file);

        MetaDataViewModel GetMetaData(FileInfo file);
    }
}