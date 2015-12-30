using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;

namespace Epub_Manager.Views.EpubData.Images
{
    /// <summary>
    /// Interaction logic for ImagesView.xaml
    /// </summary>
    public partial class ImagesView : UserControl
    {
        public ImagesView()
        {
            this.InitializeComponent();
        }

        void LayoutImagesItemsSizeChanged(object sender, ValueChangedEventArgs<Size> e)
        {
            var size = this.LayoutImages.MaximizedElementOriginalSize;

            if (!double.IsInfinity(e.NewValue.Width))
                size.Height = double.NaN;
            else
                size.Width = double.NaN;
            this.LayoutImages.MaximizedElementOriginalSize = size;
        }
    }
}
