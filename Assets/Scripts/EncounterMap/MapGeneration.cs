using System.Collections.Generic;
using UnityEngine;

namespace EncounterMap {
    /// <summary>
    /// A class representing a generation of a procedurally generated map.
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class MapGeneration : MonoBehaviour {
        /// <summary>
        /// The depth of this generation within the map generation tree.
        /// </summary>
        public int depth;

        /// <summary>
        /// References to the previous and next generations of the map.
        /// </summary>
        public MapGeneration previous, next;

        public int seed, nextSeed;

        /// <summary>
        /// The array of map nodes in this generation.
        /// </summary>
        public MapNode[] nodes;

        /// <summary>
        /// A static reference to the first generation of the map.
        /// </summary>
        private static MapGeneration genesis;

        /// <summary>
        /// Gets the first generation of the map.
        /// </summary>
        public MapGeneration Genesis {
            get {
                // If the genesis instance has already been set, return it.
                if (genesis is not null) return genesis;

                // Otherwise, search for the first generation of the map by following
                // the previous references until there are no more previous generations.
                var p = this;
                while (p.previous is not null)
                    p = p.previous;
                genesis = p;
                return genesis;
            }
            // The genesis instance can only be set internally, so it can be set
            // by the MapGeneration class itself but not by external code.
            internal set { genesis = value; }
        }

        /// <summary>
        /// Add a new map node to this generation.
        /// </summary>
        /// <param name="prefab">The prefab to use when instantiating the new node.</param>
        /// <returns>The new map node that was added.</returns>
        public MapNode AddNode(MapNode prefab) {
            // Instantiate the new node from the given prefab and add it as a child of
            // the current game object.
            var node = Instantiate(prefab, transform, true);
            node.generation = this;

            // Add the new node to the nodes array. If the array already has elements,
            // create a new List instance from the existing nodes, add the new node to
            // the list, and then convert the list back to an array. Otherwise, create
            // a new array with only the new node.
            nodes = nodes.Length > 0 ? new List<MapNode>(nodes) { node }.ToArray() : new[] { node };
            // Return the new node that was just added to the array.
            return nodes[^1];
        }
    }
}
