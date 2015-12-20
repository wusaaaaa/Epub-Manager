using Caliburn.Micro;
using Epub_Manager.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;

namespace Epub_Manager.Views.EpubData.Tree
{
    public abstract class TreeItemViewModel : PropertyChangedBase
    {

        #region Fields
        protected readonly IEventAggregator _eventAggregator;

        private bool _isExpanded;
        private bool _isSelected;
        private TreeItemViewModel _parent;
        private BindableCollection<TreeItemViewModel> _children;

        private bool _dataLoaded;
        #endregion

        #region Properties

        public abstract string DisplayText { get; }

        public abstract ImageSource Image { get; }

        public virtual string Description => null;

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set
            {
                if (this.SetProperty(ref this._isExpanded, value) && this.IsExpanded)
                {
                    if (this.Parent != null && this.Parent.IsExpanded == false)
                    {
                        this.Parent.IsExpanded = true;
                    }

                    this.ForceChildrenLoadData();
                }
            }
        }

        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                if (this.SetProperty(ref this._isSelected, value) && this.IsSelected)
                {
                    this.OnIsSelected();

                    if (this.Parent != null && this.Parent.IsExpanded == false)
                    {
                        this.Parent.IsExpanded = true;
                    }
                }
            }
        }

        public TreeItemViewModel Parent
        {
            get { return this._parent; }
            private set { this.SetProperty(ref this._parent, value); }
        }

        public BindableCollection<TreeItemViewModel> Children
        {
            get { return this._children; }
            set
            {
                var old = this._children;
                if (this.SetProperty(ref this._children, value))
                {
                    if (this.Children != null)
                        this.Children.CollectionChanged += this.ChildrenChanged;

                    if (old != null)
                        old.CollectionChanged -= this.ChildrenChanged;
                }
            }
        }
        #endregion

        #region Constructors
        protected TreeItemViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            this.Children = new BindableCollection<TreeItemViewModel>();

            this.AllowContentToChange = true;
        }
        #endregion



        public bool AllowContentToChange { get; set; }

        public virtual Dictionary<string, object> AdditionalData { get; protected set; }

        #region Methods
        protected virtual void OnIsSelected()
        {
            this._eventAggregator.PublishOnUIThread(new TreeItemSelected(this));
        }

        public virtual void LoadData()
        {
        }

        public T GetParent<T>() where T : TreeItemViewModel
        {
            TreeItemViewModel parent = this;
            while (parent != null)
            {
                if (parent is T)
                    break;

                parent = parent.Parent;
            }

            return parent as T;
        }

        public virtual void DetailsRequested(object data)
        {

        }
        #endregion

        #region Private Methods

        protected void ForceChildrenLoadData()
        {
            if (this._dataLoaded == false)
            {
                foreach (var child in this.Children)
                {
                    child.LoadData();
                }

                this._dataLoaded = true;
            }
        }

        private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            foreach (var child in e.NewItems.OfType<TreeItemViewModel>())
            {
                child.Parent = this;
            }
        }
        #endregion
    }
}
