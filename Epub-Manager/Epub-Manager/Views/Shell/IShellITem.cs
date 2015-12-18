namespace Epub_Manager.Views.Shell
{
    public interface IShellItem : Caliburn.Micro.IChild
    {

    }

    public static class ShellItemExtensions
    {
        public static ShellViewModel GetShell(this IShellItem self)
        {
            return self.Parent as ShellViewModel;
        }
    }
}