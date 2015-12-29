using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Ribbon;
using Epub_Manager.Views.Shell;
using System;
using System.Windows;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace Epub_Manager.Views.EpubData
{
    /// <summary>
    /// Interaction logic for EpubDataView.xaml
    /// </summary>
    public partial class EpubDataView : UserControl, IHaveRibbonToMerge
    {
        public RibbonControl RibbonControl => this.ActualRibbonControl;

        public EpubDataView()
        {
            this.InitializeComponent();
        }

        private void TreeItemOnSelected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
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

    public class ImageContainer : ContentControlBase
    {
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.Controller.IsMouseLeftButtonDown)
            {
                var layoutControl = this.Parent as FlowLayoutControl;
                if (layoutControl != null)
                {
                    this.Controller.IsMouseEntered = false;
                    layoutControl.MaximizedElement = layoutControl.MaximizedElement == this ? null : this;
                }
            }
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (!double.IsNaN(this.Width) && !double.IsNaN(this.Height))
            {
                if (Math.Abs(e.NewSize.Width - e.PreviousSize.Width) > double.Epsilon)
                    this.Height = double.NaN;
                else
                    this.Width = double.NaN;
            }
        }
    }
}
