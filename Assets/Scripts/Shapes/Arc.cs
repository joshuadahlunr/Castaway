using System.Collections.Generic;
using UnityEngine;

namespace Shapes {
	[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class Arc : MonoBehaviour {
		public int resolution = 32;
		public float width;
		public Transform start, end;

		// private MeshRenderer renderer;
		private MeshFilter filter;

		private void Awake() {
			// renderer = GetComponent<MeshRenderer>();
			filter = GetComponent<MeshFilter>();
		}

		private void Update() {
			RegenerateCurve();
		}

		public void RegenerateCurve() {
			var height = (start.transform.position - end.transform.position).magnitude / 2;

			int lastTopIndex = -1, lastBottomIndex = -1;
			List<Vector3> vertices = new();
			List<Vector2> UVs = new();
			List<int> indices = new();

			for (var i = 0; i < resolution; i++) {
				// Variable tracking how far along the curve we currently are
				var t = (float)i / resolution;

				// Generate a plane centered at a point along a parabola facing the camera.
				var scale = transform.lossyScale;
				var center = SampleParabola(start.transform.position, end.transform.position, height, t);
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

				// {
				// 	var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// 	go.transform.localScale = new Vector3(.1f, .1f, .1f);
				// 	go.transform.position = top;
				// }
				// {
				// 	var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// 	go.transform.localScale = new Vector3(.1f, .1f, .1f);
				// 	go.transform.position = bottom;
				// }
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
			}
			else {
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