using UnityEngine;

namespace Shapes {
	/// <summary>
	/// Class which renders an arrow composed of an arc with a cone at its end 
	/// </summary>
	/// <author>Joshua Dahl</author>
	[RequireComponent(typeof(Arc))]
	public class Arrow : MonoBehaviour {
		public GameObject start, end;
		public Arc arc;
		public Cone cone;
	
		// On start, find/create the necessary children and configure them (if nessicary)
		private void Awake() {
			start ??= new GameObject { name = "start", transform = { parent = transform } };
			end ??= new GameObject { name = "end", transform = { parent = transform } };

			arc = GetComponent<Arc>();
			cone = end.GetComponent<Cone>();
			// If the cone doesn't already exist, add it and configure it
			if (cone is null) {
				// Configure the cone
				cone = end.AddComponent<Cone>();
				cone.resolution = arc.resolution;
				cone.radiusBottom = .5f;
				cone.RegenerateCone();
				
				// Link the cone's material to the the arrow's material
				end.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
			}
		}

		// Regenerate both the arc and cone meshes at the same time
		public void Regenerate() {
			arc.RegenerateCurve();
			cone.RegenerateCone();
		}
	}
}
