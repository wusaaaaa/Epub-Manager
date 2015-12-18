using DevExpress.Xpf.Core;

namespace Epub_Manager.Views.Shell
{
    public interface IViewWithLoading
    {
        WaitIndicator WaitIndicator { get; }
    }
}