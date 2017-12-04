namespace UFZ.Interaction
{
	using Slash.Unity.DataBind.Core.Data;

	/// <summary>
	///   Item context for the items in the Collection example.
	/// </summary>
	public class ViewpointCollectionItemContext : Context
	{
		#region Fields

		private readonly Property<Viewpoint> _viewpointProperty = new Property<Viewpoint>();

		#endregion

		#region Properties

		/// <summary>
		///   Text data.
		/// </summary>
		public string Text
		{
			get {
				return this._viewpointProperty.Value.Name;
			}
			set {
				this._viewpointProperty.Value.Name = value;
			}
		}
		public Viewpoint Viewpoint
		{
			get { return this._viewpointProperty.Value; }
			set { this._viewpointProperty.Value = value; }
		}

		#endregion

		#region Commands

		public void Move()
		{
			_viewpointProperty.Value.Move();
		}

		#endregion
	}
}
