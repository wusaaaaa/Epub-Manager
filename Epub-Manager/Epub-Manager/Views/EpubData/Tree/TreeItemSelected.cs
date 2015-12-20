namespace Epub_Manager.Views.EpubData.Tree
{
    public class TreeItemSelected
    {
        public TreeItemViewModel TreeItem { get; private set; }

        public TreeItemSelected(TreeItemViewModel treeItem)
        {
            this.TreeItem = treeItem;
        }
    }
}