using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;

namespace Epub_Manager.Views.EpubData.Images
{
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