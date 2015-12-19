using DevExpress.Xpf.Ribbon;
using Epub_Manager.Views.Shell;
using System.Windows.Controls;

namespace Epub_Manager.Views.MetaData
{
    /// <summary>
    /// Interaction logic for MetaDataView.xaml
    /// </summary>
    public partial class MetaDataView : UserControl, IHaveRibbonToMerge
    {
        public MetaDataViewModel ViewModel => this.DataContext as MetaDataViewModel;
        public RibbonControl RibbonControl => this.ActualRibbonControl;

        public MetaDataView()
        {
            InitializeComponent();
        }
    }
}
