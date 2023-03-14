using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CardBattle;
using Random = UnityEngine.Random;

namespace EncounterMap {
    /// <summary>
    /// Represents a single node in the encounter map.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class MapNode : MonoBehaviour {
        /// <summary>
        /// The MapGeneration script used for generating the map.
        /// </summary>
        public MapGeneration generation;
        /// <summary>
        /// The parent nodes of this node.
        /// </summary>
        public MapNode[] parents;
        /// <summary>
        /// The child nodes of this node.
        /// </summary>
        public MapNode[] children;

        /// <summary>
        /// The depth of this node in the map, used for calculating difficulty.
        /// </summary>
        public int Depth => generation?.depth ?? -1;
        /// <summary>
        /// The positions of the parent nodes.
        /// </summary>
        public Vector3[] parentPositions => parents.Select(x => x.transform.position).ToArray();
        /// <summary>
        /// The distances between this node and its parent nodes.
        /// </summary>
        public float[] parentDistances => parentPositions.Select(x => (x - transform.position).magnitude).ToArray();

        /// <summary>
        /// The sprite used for the battle event.
        /// </summary>
        public Sprite battleSprite;
        /// <summary>
        /// The sprite used for the crewmate event.
        /// </summary>
        public Sprite crewmateSprite;
        /// <summary>
        /// The sprite used for the random event.
        /// </summary>
        public Sprite randomSprite;

        /// <summary>
        /// The type of event that occurs on this node.
        /// </summary>
        private int nodeType;
        /// <summary>
        /// A value used to randomly determine the type of event that occurs on this node.
        /// </summary>
        private int randomNode;

        /// <summary>
        /// Unity method called when the object is created.
        /// </summary>
        public void Awake() {
            // Set the type of event that occurs on this node
            SetNode();
            // Determine the sprite displayed based on the event type
            DetermineSprite(nodeType);
            // If the node is a random event, set the type of event that will occur
            if (nodeType == 1) {
                SetRandomEvent();
            }
            // If the node is a battle event, pass the depth to the CardGameManager for difficulty calculation
            if (nodeType == 0) {
                CardGameManager.encounterDifficulty = Depth;
            }
            // Print the depth of the node to the console
            Debug.Log($"{Depth}");
        }

        /// <summary>
        /// The LineRenderer component for the node's lines.
        /// </summary>
        private LineRenderer line;

        /// <summary>
        /// Initializes a LineRenderer component for the node's lines.
        /// </summary>
        /// <param name="dashedLinePrefab">The LineRenderer prefab to use for the dashed line.</param>
        public void InitLine(LineRenderer dashedLinePrefab) {
            line = GetComponent<LineRenderer>();
            StartCoroutine(SetupLinesNextFrame(dashedLinePrefab));
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="child">The child node to add.</param>
        public void AddChild(MapNode child) {
            // If children is null, create a new array with one element containing the child
            children = children is null ? new[] { child } : new List<MapNode>(children) { child }.ToArray();
            child.parents = child.parents is null ? new[] { this } : new List<MapNode>(child.parents) { this }.ToArray();
        }

        /// <summary>
        /// A coroutine function that sets up a LineRenderer with positions and texture scales based on the current object and its parent objects.
        /// </summary>
        /// <param name="dashedLinePrefab">The LineRenderer prefab to use for creating dashed lines.</param>
        private IEnumerator SetupLinesNextFrame(LineRenderer dashedLinePrefab) {
            // Yielding null means that the function will wait until the next frame to continue executing
            yield return null;

            // Check if the parents array is empty
            if (parents is null || parents.Length == 0) {
                // If it is, set the position count of the line to 2 and set the positions to two vectors with a value of Vector3.one multiplied by 1000
                line.positionCount = 2;
                line.SetPositions(new[] { Vector3.one * 1000, Vector3.one * 1000 });
            } else {
                // If there are parents, set the position count of the line to 2 and set the positions to the current object's position and the position of the first parent object
                line.positionCount = 2;
                line.SetPositions(new[] { transform.position, parents[0].transform.position });

                // Set the main texture scale of the line material to a Vector2 with an x value equal to the distance between the current object and the first parent object divided by 5, and a y value of 1
                line.material.mainTextureScale = new Vector2((parents[0].transform.position - transform.position).magnitude / 5, 1.0f);

                // For each parent object after the first, instantiate a new LineRenderer from the dashedLinePrefab and add it as a child of the current object
                foreach (var parent in parents.Skip(1)) {
                    var lineRenderer = Instantiate(dashedLinePrefab, transform);
                    // Set the position count of the new LineRenderer to 2 and set the positions to the current object's position and the position of the current parent object
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPositions(new[] { transform.position, parent.transform.position });

                    // Set the main texture scale of the new LineRenderer's material to a Vector2 with an x value equal to the distance between the current object and the current parent object divided by 5, and a y value of 1
                    lineRenderer.material.mainTextureScale = new Vector2((parent.transform.position - transform.position).magnitude / 5, 1.0f);
                }
            }
        }


        // Determines the type of node/event
        // Spawn with probability
        public void SetNode() {
            int value = Random.Range(0, 9);
            // If the value is 0-5, the event node type is Battle
            // 60% spawn rate
            if (value <= 5) {
                Debug.Log("Battle");
                nodeType = 0;
            } else if (value > 5 || value <= 7) {
                // If the value is 5-7, the event node type is Random
                // 30% spawn rate
                Debug.Log("Random");
                nodeType = 1;
            } else if (value > 7 || value <= 9) {
                // If the value is 8-9 the event node type is Crewmate
                // 10% spawn rate (can occur as a random event)
                Debug.Log("Crewmate");
                nodeType = 2;
            } else {
                nodeType = 0;
            }
        }

        // Takes value from SetNode() to determine what sprite should be displayed
        public void DetermineSprite(int type) {
            switch (type) {
                case 0:
                    this.GetComponent<SpriteRenderer>().sprite = battleSprite;
                    break;
                case 1:
                    this.GetComponent<SpriteRenderer>().sprite = randomSprite;
                    break;
                case 2:
                    this.GetComponent<SpriteRenderer>().sprite = crewmateSprite;
                    break;
                default:
                    this.GetComponent<SpriteRenderer>().sprite = battleSprite;
                    break;
            }
        }

        // Random Event has the possibility of being a Crewmate or Battle event
        public void SetRandomEvent() {
            int randomVal = Random.Range(0, 9);
            // If the value is 0-6, the event node type is Random
            // 70% spawn rate
            if  (randomVal <= 6) {
                Debug.Log("Random");
                nodeType = 1;
            } else if (randomVal > 6 || randomVal <= 7){
            // If the value is 7, the event node type is Crewmate
            // 10% spawn rate
                Debug.Log("Crewmate - Random Subevent");
                nodeType = 2;
            } else if (randomVal > 7 || randomVal <= 9){
            // If the value is 8-9 the event node type is Battle
            // 20% spawn rate (can occur as a random event)
                Debug.Log("Battle - Random Subevent");
                nodeType = 0;
            } else {
                nodeType = 1;
            }
        }

        public void SetScene() {
            if (nodeType == 1) {
                SceneManager.LoadScene("RandomScene");
            } else if (nodeType == 2) {
                SceneManager.LoadScene("CrewmateEncounterScene");
            } else if (nodeType == 0) {
                SceneManager.LoadScene("BattleScene");
            } else {
                SceneManager.LoadScene("BattleScene");
            }
        }
    }
}