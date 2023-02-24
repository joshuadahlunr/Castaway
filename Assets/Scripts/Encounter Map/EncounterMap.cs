using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// </summary>
/// <author>Jared White & Joshua Dahl</author>
public class EncounterMap : MonoBehaviour {
	[FormerlySerializedAs("whiteDot")] public MapNode whiteDotPrefab;
	public MapGeneration mapGenerationPrefab;
	public LineRenderer dashedLinePrefab;

	public MapGeneration currentGeneration;


	// Start is called before the first frame update
	private void Start() {
		// Generate 5 generations at the start
		for(var i = 0; i < 5; i++)
			GenerateNextGeneration();
	}

	public void GenerateNextGeneration() {
		// If there are no generations... create a root node!
		if (currentGeneration is null) {
			currentGeneration = Instantiate(mapGenerationPrefab, transform);
			currentGeneration.gameObject.name = "Genesis";

			currentGeneration.depth = 0;
			var root = currentGeneration.AddNode(whiteDotPrefab);
			root.transform.localPosition = Vector3.zero;
			root.transform.position = new Vector3(root.transform.position.x - 100, root.transform.position.y, root.transform.position.z - 10);
			root.InitLine(dashedLinePrefab);

			return;
		}

		// Construct a table representing how likely each type of branching
		var flatten = new int[3][];
		flatten[0] = new int [1] { 2 }.Replicate(7).ToArray(); // 2 = 7/12
		flatten[1] = new int [1] { 3 }.Replicate(4).ToArray(); // 3 = 4/12
		flatten[2] = new int [1] { 4 }.Replicate(2).ToArray(); // 4 = 2/12
		var probability = flatten.SelectMany(x => x).ToArray();

		// Create the holder for the next generation
		var nextGeneration = Instantiate(mapGenerationPrefab, currentGeneration.transform);
		nextGeneration.previous = currentGeneration;
		currentGeneration.next = nextGeneration;
		nextGeneration.depth = currentGeneration.depth + 1;

		// For each node in the current (previous) generation
		foreach (var (node, index) in currentGeneration.nodes.WithIndex()) {
			// Find its position
			var root = node.transform.position;

			// Pick a random number of children...
			int numChildren = probability[Random.Range(0, probability.Length)];
			float angle = 90 - 90.0f / numChildren + 30 * (currentGeneration.nodes.Length / 2 - index);
			for (int i = 0; i < numChildren; i++) {
				// Add each one to the graph
				var child = nextGeneration.AddNode(whiteDotPrefab);
				node.AddChild(child);

				// Positioned in an array around their parent
				float distance = Random.Range(30, 50); // TODO: Randomly generate
				child.transform.position = root + Vector3.right * Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * distance * 3 // We multiply x by 3, so their is a bias towards moving forward
				                           + Vector3.up * Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

				// If the Y goes out of bounds clamp it back in
				if (Mathf.Abs(child.transform.position.y) > 200)
					child.transform.position = new Vector3(child.transform.position.x,
						Mathf.Sign(child.transform.position.y) * -50 + child.transform.position.y, child.transform.position.z);

				angle -= 90.0f / (numChildren - 1);
			}
		}

		// If there are more than 40 nodes within a generation... remove every 5th node until there are less than 40
		while (nextGeneration.nodes.Length > 40) {
			var list = new List<MapNode>(nextGeneration.nodes);
			for(int i = 0, count = 0; i < list.Count; i++, count++)
				if (count % 5 == 1) {
					DestroyImmediate(list[i].gameObject);
					list.RemoveAt(i);
					i--;
				}
			nextGeneration.nodes = list.ToArray();
		}

		// For every node which no longer has any children (dead end) add the (2-4) closest nodes in the next generation as its children
		foreach (var node in currentGeneration.nodes) {
			node.children = node.children.Where(c => c != null).ToArray();
			if (node.children.Length > 0) continue;

			// Find the first closest node
			var minDist = float.MaxValue;
			MapNode minNode = null;
			foreach (var childNode in nextGeneration.nodes) {
				var dist = (node.transform.position - childNode.transform.position).magnitude;
				if (dist < minDist) {
					minDist = dist;
					minNode = childNode;
				}
			}
			node.AddChild(minNode);

			// Find the second closest node
			minDist = float.MaxValue;
			MapNode minNode2 = null;
			foreach (var childNode in nextGeneration.nodes) {
				if (childNode == minNode) continue; // Ignore the first closest node!
				var dist = (node.transform.position - childNode.transform.position).magnitude;
				if (dist < minDist) {
					minDist = dist;
					minNode2 = childNode;
				}
			}
			node.AddChild(minNode2);
		}

		// Draw lines between each new node and its parent(s)
		foreach (var node in nextGeneration.nodes)
			node.InitLine(dashedLinePrefab);

		// Remove generations that are more than 5 generations old from memory
		if (nextGeneration.depth > 7) {
			var genesis = currentGeneration.Genesis;
			genesis.next.transform.parent = transform;
			genesis.next.previous = null;
			Destroy(genesis.gameObject);
			currentGeneration.Genesis = null;
		}

		// The current generation is now the next generation
		currentGeneration = nextGeneration;
	}




	/* TO DOs:
	    - Lerp/tween player icon to current location, remove previous icon if applicable (crewmate, battle, or random event)
	    - Random event generation within certain bounds
	    - Time movement between destinations
	    - Move between
	*/
}