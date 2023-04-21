using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CardBattle;
using Random = UnityEngine.Random;

/// <summary>
/// <author>Joshua Dahl & Jared White</author>
/// </summary>
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
        /// The sprite used for the completed events.
        /// </summary>
        public Sprite flagSprite;
        /// <summary>
        /// The different types of events
        /// </summary>
        public enum NodeType {
			Battle,
			Crewmate,
			Random
		}
        /// <summary>
        /// Holds the type of event at this node
        /// </summary>
        public NodeType nodeType;

        /// <summary>
        /// Holds the index for the current node
        /// </summary>
        public int currentIndex;

        /// <summary>
        /// Counter for the index
        /// </summary>
        private static int i;

        /// <summary>
        /// Unity method called when the object is created.
        /// </summary>
        public void Awake() {
            // Set the index of this node
            currentIndex = i;
            i = i + 1;
            // Set the type of event that occurs on this node
            SetNode();
            // Determine the sprite displayed based on the event type
            DetermineSprite(nodeType);
            // If the node is a random event, set the type of event that will occur
            if (nodeType == NodeType.Random) {
                SetRandomEvent();
            }
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
            // If the value is 0-4, the event node type is Battle
            // 50% spawn rate
            if (value <= 4) {
                nodeType = NodeType.Battle;
            } else if (value > 4 || value <= 7) {
                // If the value is 5-7, the event node type is Random
                // 30% spawn rate
                nodeType = NodeType.Random;
            } else if (value > 7 || value <= 9) {
                // If the value is 8-9 the event node type is Crewmate
                // 20% spawn rate (can occur as a random event)
                nodeType = NodeType.Crewmate;
            } else {
                nodeType = NodeType.Battle;
            }
        }

        // Takes value from SetNode() to determine what sprite should be displayed
        public void DetermineSprite(NodeType type) {
            switch (type) {
                case NodeType.Battle:
                    this.GetComponent<SpriteRenderer>().sprite = battleSprite;
                    break;
                case NodeType.Random:
                    this.GetComponent<SpriteRenderer>().sprite = randomSprite;
                    break;
                case NodeType.Crewmate:
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
                nodeType = NodeType.Random;
            } else if (randomVal > 6 || randomVal <= 7){
            // If the value is 7, the event node type is Crewmate
            // 10% spawn rate
                nodeType = NodeType.Crewmate;
            } else if (randomVal > 7 || randomVal <= 9){
            // If the value is 8-9 the event node type is Battle
            // 20% spawn rate (can occur as a random event)
                nodeType = NodeType.Battle;
            } else {
                nodeType = NodeType.Random;
            }
        }

        // Sets the scene of that node
        public void SetScene() {
            if (nodeType == NodeType.Random) {
                SceneManager.LoadScene("RandomScene");
            } else if (nodeType == NodeType.Crewmate) {
                SceneManager.LoadScene("CrewmateEncounterScene");
            } else { // This case assumes that anything not listed above is a battle node!
                SceneManager.LoadScene("BattleScene");
                // Sends the depth to the CGM to determine difficulty
                CardGameManager.encounterDifficulty = Depth;
                // MysticCharge.encounterDifficulty = Depth; 
            }
        }

        // Causes the scene to change to the relative node on collision
        public void OnTriggerEnter2D(Collider2D collision) {
            SetScene();
            Debug.Log(currentIndex);
            //EncounterMapScript.shipsChildIndex = currentIndex;
            //this.enabled = !this.enabled;
        }

        public void OnTriggerExit2D(Collider2D collision) {
            this.GetComponent<SpriteRenderer>().sprite = flagSprite;
            //EncounterMapScript.shipsChildIndex = currentIndex;
            //this.enabled = !this.enabled;
        }

        // Gets the position of the node
        public void GetNodePosition() {
            Vector3 nodePos = transform.position;
            // Sets the z position to 0 since its 2D and we don't care about position!
            nodePos.z = 0f;
            // Passes the position of the node selected to the PlayerMovement targetPos variable
            PlayerMovement.targetPos = (Vector2)nodePos;
            // Check for position
            Debug.Log(nodePos);
        }
    }
}