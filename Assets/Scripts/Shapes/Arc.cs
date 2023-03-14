using System.Collections.Generic;
using UnityEngine;

namespace Shapes {
	/// <summary>
	///     Class which generates an arc mesh
	/// </summary>
	/// <author>Joshua Dahl</author>
	[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter)), ExecuteInEditMode]
	 // Make sure arc is regenerated even in edit mode!
	public class Arc : MonoBehaviour {
		// How many segments the arc should have
		public int resolution = 32;

		// How wide the arc should be
		public float width;

		// Positions where the arc should start and end
		public Transform start, end;


		// Mesh filter to store the generated mesh in
		private MeshFilter filter;

		// Find a reference to the filter on awake
		private void Awake() => filter = GetComponent<MeshFilter>();

		// Every frame update the curve of the arc
		private void Update() { RegenerateCurve(); }

		// Function which updates the curve of the arc and generates a new mesh
		public void RegenerateCurve() {
			// The arc should be less tall than it is long
			var height = (start.transform.position - end.transform.position).magnitude / 2;

			// Index locations of the vertices in the last segment
			int lastTopIndex = -1, lastBottomIndex = -1;

			// List of vertices, UVs, and indices
			List<Vector3> vertices = new();
			List<Vector2> UVs = new();
			List<int> indices = new();

			// For every segment we should split the arc into
			for (var i = 0; i < resolution; i++) {
				// Variable tracking how far along the curve we currently are
				var t = (float)i / resolution;

				// Generate a plane centered at a point along a parabola facing the camera.
				var scale = transform.lossyScale;
				var center = SampleParabola(start.transform.position, end.transform.position, height, t);
				// Adjust te position of the plane to account for the scale of the object
				center.x /= scale.x;
				center.y /= scale.y;
				center.z /= scale.z;
				var normal = (Camera.main.transform.position - center).normalized;
				var cameraRight = Camera.main.transform.right.normalized;
				var plane = new Plane(normal, center);

				// Generate the top point (position, UV, and index)
				vertices.Add(plane.ClosestPointOnPlane(center + cameraRight * width));
				UVs.Add(new Vector2(t, 1));
				var topIndex = i * 2;
				// Generate the bottom point (position, UV, and index)
				vertices.Add(plane.ClosestPointOnPlane(center - cameraRight * width));
				UVs.Add(new Vector2(t, 0));
				var bottomIndex = topIndex + 1;

				// Add the two triangles for this section of the curve
				// (skipping the first interval since we need info about previous points)
				if (i > 1) {
					indices.Add(lastTopIndex);
					indices.Add(topIndex);
					indices.Add(bottomIndex);

					indices.Add(bottomIndex);
					indices.Add(lastBottomIndex);
					indices.Add(lastTopIndex);
				}

				// Save the indices for use in the next interval
				lastTopIndex = topIndex;
				lastBottomIndex = bottomIndex;
			}

			// Update the displayed mesh
			var mesh = new Mesh {
				vertices = vertices.ToArray(),
				uv = UVs.ToArray(),
				triangles = indices.ToArray()
			};
			mesh.RecalculateNormals();
			filter.mesh = mesh;
		}

		// From: https://forum.unity.com/threads/generating-dynamic-parabola.211681/#post-1426169
		protected Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t) {
			var parabolicT = t * 2 - 1;
			if (Mathf.Abs(start.y - end.y) < 0.1f) {
				//start and end are roughly level, pretend they are - simpler solution with less steps
				var travelDirection = end - start;
				var result = start + t * travelDirection;
				result.y += (-parabolicT * parabolicT + 1) * height;
				return result;
			} else {
				//start and end are not level, gets more complicated
				var travelDirection = end - start;
				var levelDirecteion = end - new Vector3(start.x, end.y, start.z);
				var right = Vector3.Cross(travelDirection, levelDirecteion);
				var up = Vector3.Cross(right, travelDirection);
				if (end.y > start.y) up = -up;
				var result = start + t * travelDirection;
				result += (-parabolicT * parabolicT + 1) * height * up.normalized;
				return result;
			}
		}
	}
}