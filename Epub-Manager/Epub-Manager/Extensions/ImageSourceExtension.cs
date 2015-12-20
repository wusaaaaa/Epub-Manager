using System.Drawing;
using System.Windows.Media;

namespace Epub_Manager.Extensions
{
    public static class ImageSourceExtensions
    {
        public static ImageSource FromBitmap(this Bitmap image)
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            return (ImageSource)converter.ConvertFrom(image);
        }
    }
}