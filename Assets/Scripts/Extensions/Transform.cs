using System.Collections.Generic;
using UnityEngine;

namespace Extensions {
	public static class TransformExtensions {
		
		// Changes the scale of the parent without changing the scale of any of the children!
		// From: https://answers.unity.com/questions/330906/how-to-preserve-size-of-children-when-changing-sca.html
		public static void ChangeParentScale(this Transform parent, Vector3 scale) {
			var children = new List<Transform>();
			foreach (Transform child in parent) {
				child.parent = null;
				children.Add(child);
			}

			parent.localScale = scale;
			foreach (var child in children) child.parent = parent;
		}
		
		// From: https://answers.unity.com/questions/1007585/reading-and-setting-asn-objects-global-scale-with.html
		public static void SetGlobalScale(this Transform transform, Vector3 globalScale) {
			transform.localScale = Vector3.one;
			transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
		}
	}
}