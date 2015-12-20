using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Epub_Manager.Images
{
    public static class Resources
    {
        public static ImageSource Folder => GetImage("Folder.png");

        public static ImageSource File => GetImage("File.png");

        private static ImageSource GetImage(string imageName)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/{imageName}"));
        }
    }
}