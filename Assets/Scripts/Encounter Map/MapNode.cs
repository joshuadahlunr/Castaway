using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MapNode : MonoBehaviour {
	public MapGeneration generation;
	public MapNode[] parents;
	public MapNode[] children;

	public int Depth => generation?.depth ?? -1; 

	private LineRenderer line;
	public void InitLine(LineRenderer dashedLinePrefab) {
		line = GetComponent<LineRenderer>();
		
		StartCoroutine(SetupLinesNextFrame(dashedLinePrefab));
	}

	public void AddChild(MapNode child) {
		children = children is null ? new[] { child } : new List<MapNode>(children) { child }.ToArray();
		child.parents = child.parents is null ? new[] { this } : new List<MapNode>(child.parents) { this }.ToArray();
	}

	private IEnumerator SetupLinesNextFrame(LineRenderer dashedLinePrefab) {
		yield return null;

		if (parents is null || parents.Length == 0) {
			line.positionCount = 2;
			line.SetPositions(new[] { Vector3.one * 1000, Vector3.one * 1000 });
		} else {
			line.positionCount = 2;
			line.SetPositions(new[] { transform.position, parents[0].transform.position });
			line.material.mainTextureScale = new Vector2((parents[0].transform.position - transform.position).magnitude / 5, 1.0f);
			
			foreach (var parent in parents.Skip(1)) {
				var lineRenderer = Instantiate(dashedLinePrefab, transform);
				lineRenderer.positionCount = 2;
				lineRenderer.SetPositions(new[] { transform.position, parent.transform.position });
				lineRenderer.material.mainTextureScale = new Vector2((parent.transform.position - transform.position).magnitude / 5, 1.0f);
			}
		}
		
		// line.text
		
	}
}