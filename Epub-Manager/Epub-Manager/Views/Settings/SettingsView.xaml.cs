using DevExpress.Xpf.Ribbon;
using Epub_Manager.Views.Shell;
using System.Windows.Controls;

namespace Epub_Manager.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, IHaveRibbonToMerge
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;
        public RibbonControl RibbonControl => this.ActualRibbonControl;

        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
