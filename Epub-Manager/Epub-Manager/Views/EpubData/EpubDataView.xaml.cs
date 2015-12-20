using DevExpress.Xpf.Ribbon;
using Epub_Manager.Views.Shell;
using System.Windows.Controls;

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
            InitializeComponent();
        }

    }
}
