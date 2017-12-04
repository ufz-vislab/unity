namespace UFZ.Interaction
{
	using UnityEngine;
    using System.Linq;
    using Slash.Unity.DataBind.Core.Data;

    /// <summary>
    ///   Context for Viewpoint collection.
    /// </summary>
    public class ViewpointCollectionContext : Context
    {
        private readonly Property<Collection<ViewpointCollectionItemContext>> _itemsProperty =
            new Property<Collection<ViewpointCollectionItemContext>>(new Collection<ViewpointCollectionItemContext>());

        /// <summary>
        ///   Constructor.
        /// </summary>
        public ViewpointCollectionContext()
        {
	        Init();
        }

	    private void Init()
	    {
		    Items.Clear();
		    var go = GameObject.Find("Viewpoints");
		    if (go == null) return;
		    var vps = go.GetComponentsInChildren<Viewpoint>();
		    foreach (var vp in vps)
		    	Items.Add(new ViewpointCollectionItemContext { Viewpoint = vp });
	    }

        /// <summary>
        ///   Items.
        /// </summary>
        public Collection<ViewpointCollectionItemContext> Items
        {
            get { return _itemsProperty.Value; }
            set { _itemsProperty.Value = value; }
        }

        /// <summary>
        ///   Replaces the collection completely.
        /// </summary>
        public void ReplaceCollection()
        {
            Init();
        }
    }
}
