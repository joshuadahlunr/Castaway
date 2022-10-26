using UnityEngine;

// an Editor method to create a cone primitive (so far no end caps)
// the top center is placed at (0/0/0)
// the bottom center is placed at (0/0/length)
// if either one of the radii is 0, the result will be a cone, otherwise a truncated cone
// note you will get inevitable breaks in the smooth shading at cone tips
// note the resulting mesh will be created as an asset in Assets/Editor
// From: https://gist.github.com/gszauer/5718607
// Author: Wolfram Kresse

namespace Shapes {
	[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class Cone : MonoBehaviour {
		public int resolution = 10;
		public float radiusTop;
		public float radiusBottom = 1f;
		public float length = 1f;
		// if >0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;
		public float openingAngle;
		public bool outside = true;
		public bool inside;

		// private MeshRenderer renderer;
		private MeshFilter filter;

		private void Awake() {
			// renderer = GetComponent<MeshRenderer>();
			filter = GetComponent<MeshFilter>();
		}

#if UNITY_EDITOR
		private void Update() {
			RegenerateCone();
		}
#else
		private void Start() {
			RegenerateCone();
		}
#endif

		public void RegenerateCone() {
			if (openingAngle > 0 && openingAngle < 180) {
				radiusTop = 0;
				radiusBottom = length * Mathf.Tan(openingAngle * Mathf.Deg2Rad / 2);
			}

			var mesh = new Mesh();
			var multiplier = (outside ? 1 : 0) + (inside ? 1 : 0);
			var offset = outside && inside ? 2 * resolution : 0;
			var numVertices = 2 * multiplier * resolution;
			var vertices = new Vector3[numVertices]; // 0..n-1: top, n..2n-1: bottom
			var normals = new Vector3[numVertices];
			var uvs = new Vector2[numVertices];
			int[] tris;
			var slope = Mathf.Atan((radiusBottom - radiusTop) / length); // (rad difference)/height
			var slopeSin = Mathf.Sin(slope);
			var slopeCos = Mathf.Cos(slope);
			int i;

			for (i = 0; i < resolution; i++) {
				var angle = 2 * Mathf.PI * i / resolution;
				var angleSin = Mathf.Sin(angle);
				var angleCos = Mathf.Cos(angle);
				var angleHalf = 2 * Mathf.PI * (i + 0.5f) / resolution; // for degenerated normals at cone tips
				var angleHalfSin = Mathf.Sin(angleHalf);
				var angleHalfCos = Mathf.Cos(angleHalf);

				vertices[i] = new Vector3(radiusTop * angleCos, radiusTop * angleSin, 0);
				vertices[i + resolution] = new Vector3(radiusBottom * angleCos, radiusBottom * angleSin, length);

				if (radiusTop == 0)
					normals[i] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
				else normals[i] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
				if (radiusBottom == 0)
					normals[i + resolution] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
				else normals[i + resolution] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);

				uvs[i] = new Vector2(1.0f * i / resolution, 1);
				uvs[i + resolution] = new Vector2(1.0f * i / resolution, 0);

				if (outside && inside) {
					// vertices and uvs are identical on inside and outside, so just copy
					vertices[i + 2 * resolution] = vertices[i];
					vertices[i + 3 * resolution] = vertices[i + resolution];
					uvs[i + 2 * resolution] = uvs[i];
					uvs[i + 3 * resolution] = uvs[i + resolution];
				}

				if (inside) {
					// invert normals
					normals[i + offset] = -normals[i];
					normals[i + resolution + offset] = -normals[i + resolution];
				}
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;

			// create triangles
			// here we need to take care of point order, depending on inside and outside
			var cnt = 0;
			if (radiusTop == 0) {
				// top cone
				tris = new int[resolution * 3 * multiplier];
				if (outside)
					for (i = 0; i < resolution; i++) {
						tris[cnt++] = i + resolution;
						tris[cnt++] = i;
						if (i == resolution - 1)
							tris[cnt++] = resolution;
						else
							tris[cnt++] = i + 1 + resolution;
					}

				if (inside)
					for (i = offset; i < resolution + offset; i++) {
						tris[cnt++] = i;
						tris[cnt++] = i + resolution;
						if (i == resolution - 1 + offset)
							tris[cnt++] = resolution + offset;
						else
							tris[cnt++] = i + 1 + resolution;
					}
			}
			else if (radiusBottom == 0) {
				// bottom cone
				tris = new int[resolution * 3 * multiplier];
				if (outside)
					for (i = 0; i < resolution; i++) {
						tris[cnt++] = i;
						if (i == resolution - 1)
							tris[cnt++] = 0;
						else
							tris[cnt++] = i + 1;
						tris[cnt++] = i + resolution;
					}

				if (inside)
					for (i = offset; i < resolution + offset; i++) {
						if (i == resolution - 1 + offset)
							tris[cnt++] = offset;
						else
							tris[cnt++] = i + 1;
						tris[cnt++] = i;
						tris[cnt++] = i + resolution;
					}
			}
			else {
				// truncated cone
				tris = new int[resolution * 6 * multiplier];
				if (outside)
					for (i = 0; i < resolution; i++) {
						var ip1 = i + 1;
						if (ip1 == resolution)
							ip1 = 0;

						tris[cnt++] = i;
						tris[cnt++] = ip1;
						tris[cnt++] = i + resolution;

						tris[cnt++] = ip1 + resolution;
						tris[cnt++] = i + resolution;
						tris[cnt++] = ip1;
					}

				if (inside)
					for (i = offset; i < resolution + offset; i++) {
						var ip1 = i + 1;
						if (ip1 == resolution + offset)
							ip1 = offset;

						tris[cnt++] = ip1;
						tris[cnt++] = i;
						tris[cnt++] = i + resolution;

						tris[cnt++] = i + resolution;
						tris[cnt++] = ip1 + resolution;
						tris[cnt++] = ip1;
					}
			}

			mesh.triangles = tris;
			filter.mesh = mesh;
		}
	}
}