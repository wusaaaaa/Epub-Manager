using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Windows;

namespace Epub_Manager.Windows
{
    public class WindowSettings : Dictionary<string, object>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        private WindowSettings()
        {
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Allows the user to create a new <see cref="WindowSettings"/> with the following settings.
        /// </summary>
        public static WindowSettings With()
        {
            return new WindowSettings();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures the window to autosize to fit it's content.
        /// </summary>
        public WindowSettings AutoSize()
        {
            this[Window.SizeToContentProperty.Name] = SizeToContent.WidthAndHeight;

            return this;
        }
        /// <summary>
        /// Configures the window to have the specified size.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public WindowSettings FixedSize(int width, int height)
        {
            this[FrameworkElement.WidthProperty.Name] = width;
            this[FrameworkElement.HeightProperty.Name] = height;
            this[FrameworkElement.MinWidthProperty.Name] = width;
            this[FrameworkElement.MinHeightProperty.Name] = height;

            return this;
        }
        /// <summary>
        /// Configures the window to allow resizing.
        /// </summary>
        public WindowSettings Resize()
        {
            this[Window.ResizeModeProperty.Name] = ResizeMode.CanResize;

            return this;
        }
        /// <summary>
        /// Configures the window to disallow resizing.
        /// </summary>
        public WindowSettings NoResize()
        {
            this[Window.ResizeModeProperty.Name] = ResizeMode.NoResize;

            return this;
        }

        /// <summary>
        /// Configures the window to not have window buttons.
        /// </summary>
        public WindowSettings NoWindowButtons()
        {
            this[ExtendedDxRibbonWindow.HideWindowButtonsProperty.Name] = true;

            return this;
        }
        /// <summary>
        /// Configures the window to not have a icon.
        /// </summary>
        public WindowSettings NoIcon()
        {
            this[DXWindow.ShowIconProperty.Name] = false;

            return this;
        }
        #endregion
    }
}