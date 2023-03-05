using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// </summary>
/// <author>Joshua Dahl</author>
namespace EncounterMap {
	public class MapGeneration: MonoBehaviour {
		public int depth;
		public MapGeneration previous, next;
		
		public MapNode[] nodes;

		private static MapGeneration genesis;
		public MapGeneration Genesis {
			get {
				if (genesis is not null) return genesis;
				
				var p = this;
				while (p.previous is not null)
					p = p.previous;
				genesis = p;
				return genesis;
			}
			internal set { genesis = value; }
		}

		public MapNode AddNode(MapNode prefab) {
			var node = Instantiate(prefab, transform, true);
			
			nodes = nodes.Length > 0 ? new List<MapNode>(nodes) { node }.ToArray() : new[] { node };
			return nodes[^1];
		}

	}
}