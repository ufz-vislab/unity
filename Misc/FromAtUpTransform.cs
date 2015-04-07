using UnityEngine;

namespace UFZ.Misc
{
	/// <summary>
	/// Sets the transform of the GameObject to the given From, At and Up vectors
	/// </summary>
	/// Useful for converting VRED viewpoints to Unity coordinates.
	public class FromAtUpTransform : MonoBehaviour
	{
		/// <summary>
		/// Is z the up-vector, else y.
		/// </summary>
		public bool ZUp = false;
		public Vector3 From;
		public Vector3 At;
		public Vector3 Up;

		private void OnValidate()
		{
			if (ZUp)
			{
				gameObject.transform.position = new Vector3(From.x, From.z, From.y);
				gameObject.transform.LookAt(new Vector3(At.x, At.z, At.y), new Vector3(Up.x, Up.z, Up.y));
			}
			else
			{
				gameObject.transform.position = From;
				gameObject.transform.LookAt(At, Up);
			}
		}
	}
}
