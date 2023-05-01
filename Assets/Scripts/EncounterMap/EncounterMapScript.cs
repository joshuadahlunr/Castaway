using System.Collections.Generic;
using System.Linq;
using Extensions;
using JetBrains.Annotations;
using SQLite;
using UnityEngine;
using UnityEngine.Serialization;

namespace EncounterMap {
	/// <summary>
	///     This class represents an encounter map generator that generates multiple generations of nodes
	///     with randomized branching based on a predefined probability table.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class EncounterMapScript : MonoBehaviour {
		/// <summary>
		///     Represents information about a game's encounter map for save purposes.
		/// </summary>
		public class EncounterMapInfo {
			private static int trash; // A private static variable that is used to ignore id values

			/// <summary>
			///     Gets or sets the primary key of this record in the database.
			/// </summary>
			[PrimaryKey]
			public int id {
				get => 0;
				set => trash = value;
			}

			/// <summary>
			///     Gets or sets the current seed value of the encounter map.
			/// </summary>
			public int currentSeed { get; set; }

			/// <summary>
			///     Gets or sets the next seed value of the encounter map.
			/// </summary>
			public int nextSeed { get; set; }

			/// <summary>
			///     Gets or sets the depth of the encounter map.
			/// </summary>
			public int depth { get; set; }

			/// <summary>
			///     Gets or sets the index of the ships child node in the encounter map.
			/// </summary>
			public int shipsChildIndex { get; set; }

			/// <summary>
			///     An array of Vector3 objects that stores the coordinates of all parent node positions.
			///     This property is not added to the database.
			/// </summary>
			public Vector3[] parentNodePositions;

			/// <summary>
			///     Loads the current encounter map information from the database.
			/// </summary>
			/// <returns>An instance of the EncounterMapInfo class representing the current encounter map information, or null if no matching record is found.</returns>
			public static EncounterMapInfo LoadCurrent() {
				var @out = DatabaseManager.GetOrCreateTable<EncounterMapInfo>().First();
				if (@out is null) return null;

				@out.parentNodePositions = DatabaseManager.GetOrCreateTable<EncounterMapParentPositions>()
					.Select(db => new Vector3(db.x, db.y, db.z)).ToArray();
				return @out;
			}
		}

		/// <summary>
		///     Represents the coordinates of a parent node in a game's encounter map.
		/// </summary>
		public class EncounterMapParentPositions {
			/// <summary>
			///     Gets or sets the primary key of this record in the database.
			/// </summary>
			[PrimaryKey]
			public int id { get; set; }

			/// <summary>
			///     Gets or sets the x-coordinate of the parent node.
			/// </summary>
			public float x { get; set; }

			/// <summary>
			///     Gets or sets the y-coordinate of the parent node.
			/// </summary>
			public float y { get; set; }

			/// <summary>
			///     Gets or sets the z-coordinate of the parent node.
			/// </summary>
			public float z { get; set; }
		}


		/// <summary>
		///     The prefab representing the white dot that is used to construct map nodes.
		/// </summary>
		[FormerlySerializedAs("whiteDot")] public MapNode whiteDotPrefab;

		/// <summary>
		///     The prefab representing the map generation object used to generate nodes.
		/// </summary>
		public MapGeneration mapGenerationPrefab;

		/// <summary>
		///     The prefab representing the dashed line object used to draw lines between nodes.
		/// </summary>
		public LineRenderer dashedLinePrefab;

		/// <summary>
		///     The current map generation instance.
		/// </summary>
		public MapGeneration currentGeneration;

		// Start is called before the first frame update
		private void Start() {
			// Play calm music
			AudioManager.instance.PlayCalmMusic();
			
			// If we have information saved in the database, then generate based on the saved information
			if (DatabaseManager.GetOrCreateTable<EncounterMapInfo>().Any())
				GenerateNextGeneration(DatabaseManager.GetOrCreateTable<EncounterMapInfo>().First());
			else {
				// Testing... save the first generation
				GenerateNextGeneration();
				SaveCurrentGeneration();
			}

			// Generate 5 generations at the start
			for (var i = 0; i < 5; i++)
				GenerateNextGeneration();
		}

		/// <summary>
		/// Saves the current generation of an encounter map to the database.
		/// </summary>
		/// <remarks>
		/// This method deletes all existing entries in the EncounterMapInfo and EncounterMapParentPositions tables and creates new entries based on the current generation of the encounter map.
		/// </remarks>
		public void SaveCurrentGeneration() {
			{
				var table = DatabaseManager.GetOrCreateTable<EncounterMapInfo>();
				table.Delete(_ => true); // Clear the table (delete all entries)
				DatabaseManager.database.Insert(new EncounterMapInfo {
					id = 0,
					currentSeed = currentGeneration.seed,
					nextSeed = currentGeneration.nextSeed,
					depth = currentGeneration.depth,
					shipsChildIndex =
						0 // TODO: @Jared I left this value so you can specify where the ship is... if you have a better method than the child node index then feel free to change it!
				});
			}
			{
				var table = DatabaseManager.GetOrCreateTable<EncounterMapParentPositions>();
				table.Delete(_ => true); // Clear the table (delete all entries)
				if (currentGeneration.previous is null) return;
				foreach (var (node, i) in currentGeneration.previous.nodes.WithIndex())
					DatabaseManager.database.Insert(new EncounterMapParentPositions {
						id = i,
						x = node.transform.position.x,
						y = node.transform.position.y,
						z = node.transform.position.z
					});
			}
		}

		/// <summary>
		///     Generates the next map generation instance with randomized branching based on a predefined probability table.
		/// </summary>
		/// <param name="info">Optional parameter representing the </param>
		public void GenerateNextGeneration([CanBeNull] EncounterMapInfo info = null) {
			// Construct a table representing how likely each type of branching is
			var flatten = new int[3][];
			flatten[0] = new int [1] { 2 }.Replicate(7).ToArray(); // 2 = 7/12
			flatten[1] = new int [1] { 3 }.Replicate(4).ToArray(); // 3 = 4/12
			flatten[2] = new int [1] { 4 }.Replicate(2).ToArray(); // 4 = 2/12
			var probability = flatten.SelectMany(x => x).ToArray();

			// Save the state of the random number generator
			var rState = Random.state;
			try {
				// If there are no generations... create a root node!
				if (currentGeneration is null) {
					currentGeneration = Instantiate(mapGenerationPrefab, transform);
					currentGeneration.gameObject.name = "Genesis";
					currentGeneration.seed = RandomInt();
					Random.InitState(currentGeneration.seed);
					currentGeneration.nextSeed = RandomInt();


					if (info is null || (info.parentNodePositions?.Length ?? 0) == 0) {
						currentGeneration.depth = 0;
						if (info is not null) {
							currentGeneration.seed = info.currentSeed;
							currentGeneration.nextSeed = info.nextSeed;
							currentGeneration.depth = info.depth;
						}

						var root = currentGeneration.AddNode(whiteDotPrefab);
						root.transform.localPosition = Vector3.zero;
						root.transform.position = new Vector3(root.transform.position.x - 100,
							root.transform.position.y, root.transform.position.z - 10);
						root.InitLine(dashedLinePrefab);

						return;
					} else {
						// We have been given initialization info
						currentGeneration.depth = info.depth - 1;
						// We have to create dummy nodes for the parent generation that we can then spawn the current generation of nodes off of...
						foreach (var position in info.parentNodePositions) {
							var node = currentGeneration
								.AddNode(
									whiteDotPrefab); // TODO: Should we have a separate dummy prefab for these nodes?
							node.transform.position = position;
							node.InitLine(dashedLinePrefab);
						}
					}
				}


				// Create the holder for the next generation
				var nextGeneration = Instantiate(mapGenerationPrefab, currentGeneration.transform);
				nextGeneration.previous = currentGeneration;
				currentGeneration.next = nextGeneration;
				nextGeneration.depth = currentGeneration.depth + 1;
				nextGeneration.seed = currentGeneration.nextSeed;
				Random.InitState(nextGeneration.seed);
				nextGeneration.nextSeed = RandomInt();

				if (info is not null) {
					nextGeneration.depth = info.depth; // Not strictly necessary but makes that it occurs more clear!
					nextGeneration.seed = info.currentSeed;
					Random.InitState(nextGeneration.seed);
					nextGeneration.nextSeed = info.nextSeed;
					// TODO: @Jared deal with ship here maybe?
				}

				// For each node in the current (previous) generation
				foreach (var (node, index) in currentGeneration.nodes.WithIndex()) {
					// Find its position
					var root = node.transform.position;

					// Pick a random number of children...
					var numChildren = probability[Random.Range(0, probability.Length)];
					var angle = 90 - 90.0f / numChildren + 30 * (currentGeneration.nodes.Length / 2 - index);
					for (var i = 0; i < numChildren; i++) {
						// Add each one to the graph
						var child = nextGeneration.AddNode(whiteDotPrefab);
						node.AddChild(child);

						// Positioned in an array around their parent
						float distance = Random.Range(30, 50); // TODO: Randomly generate
						child.transform.position =
							root + Vector3.right * Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * distance *
							     3 // We multiply x by 3, so their is a bias towards moving forward
							     + Vector3.up * Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

						// If the Y goes out of bounds clamp it back in
						if (Mathf.Abs(child.transform.position.y) > 200)
							child.transform.position = new Vector3(child.transform.position.x,
								Mathf.Sign(child.transform.position.y) * -50 + child.transform.position.y,
								child.transform.position.z);

						angle -= 90.0f / (numChildren - 1);
					}
				}

				// If there are more than 40 nodes within a generation... remove every 5th node until there are less than 40
				while (nextGeneration.nodes.Length > 40) {
					var list = new List<MapNode>(nextGeneration.nodes);
					for (int i = 0, count = 0; i < list.Count; i++, count++)
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
			} finally {
				// Once we are done reset the random number generate to the same state as when we started generating!
				Random.state = rState;
			}
		}

		public int RandomInt() => Random.Range(int.MinValue, int.MaxValue);

		public MapGeneration[] EnumerateGenerations(){
			var generations = new List<MapGeneration> { currentGeneration };
			MapGeneration cur = currentGeneration.previous;
			while(cur != null){
				generations.Add(cur);
				cur = cur.previous;
			}

			cur = currentGeneration.next;
			while(cur != null){
				generations.Add(cur);
				cur = cur.next;
			}

			return generations.ToArray();
		}

		public MapNode[] EnumerateNodes() {
			var generations = EnumerateGenerations();

			return generations.SelectMany(gen => gen.nodes).ToArray();
		}

		public MapNode ClosestNode(Vector3 pos) {
			var nodes = EnumerateNodes();

			var closestDist = Mathf.Infinity;
			MapNode closest = null;
			foreach(var node in nodes){
				var dist = (node.transform.position - pos).magnitude;
				if (!(dist < closestDist)) continue;
				closestDist = dist;
				closest = node;
			}
			return closest;
		}

	}
}