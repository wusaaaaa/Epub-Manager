using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon;
using System.Windows;
using System.Windows.Controls;

namespace Epub_Manager.Windows
{
    public class ExtendedDxRibbonWindow : DXRibbonWindow
    {
        #region Dependency Properties
        public static readonly DependencyProperty HideWindowButtonsProperty = DependencyProperty.Register(
            "HideWindowButtons", typeof(bool), typeof(ExtendedDxRibbonWindow), new PropertyMetadata(default(bool)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the window buttons should be hidden.
        /// </summary>
        public bool HideWindowButtons
        {
            get { return (bool)this.GetValue(HideWindowButtonsProperty); }
            set { this.SetValue(HideWindowButtonsProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDxRibbonWindow"/> class.
        /// </summary>
        public ExtendedDxRibbonWindow()
        {
            this.Loaded += this.OnLoaded;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Called when the <see cref="FrameworkElement.Loaded"/> event is executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="routedEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Loaded -= this.OnLoaded;

            if (this.HideWindowButtons)
            {
                var stackPanel = LayoutHelper.FindElementByName(this, "stackPanel") as StackPanel;
                if (stackPanel != null)
                {
                    stackPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion
    }
}