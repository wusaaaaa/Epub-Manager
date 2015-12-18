using Caliburn.Micro;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;

namespace Epub_Manager.Views.Shell
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl, IViewWithLoading
    {
        public ShellViewModel ViewModel => this.DataContext as ShellViewModel;
        public WaitIndicator WaitIndicator => this.ActualWaitIndicator;

        public ShellView()
        {
            this.InitializeComponent();
        }

        private void ShellViewOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ViewModel.ActivationProcessed += this.ViewModelOnActivationProcessed;
            this.HandleRibbonMergeAndUnMerge();
        }

        private void ViewModelOnActivationProcessed(object sender, ActivationProcessedEventArgs activationProcessedEventArgs)
        {
            this.HandleRibbonMergeAndUnMerge();
        }

        private void HandleRibbonMergeAndUnMerge()
        {
            this.ActualRibbonControl.UnMerge();

            var hasRibbonToMerge = this.ActiveItem.Content as IHaveRibbonToMerge;
            if (hasRibbonToMerge != null)
            {
                this.ActualRibbonControl.Merge(hasRibbonToMerge.RibbonControl);
            }
        }
    }
}
