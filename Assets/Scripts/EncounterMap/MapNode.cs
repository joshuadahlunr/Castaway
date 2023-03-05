using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CardBattle;
using Random = UnityEngine.Random;

namespace EncounterMap {
    [RequireComponent(typeof(LineRenderer))]
    public class MapNode : MonoBehaviour {
        public MapGeneration generation;
        public MapNode[] parents;
        public MapNode[] children;

        public int Depth => generation?.depth ?? -1; // Used to calculate difficulty
        public Vector3[] parentPositions => parents.Select(x => x.transform.position).ToArray();
        public float[] parentDistances => parentPositions.Select(x => (x - transform.position).magnitude).ToArray();
        
        public Sprite battleSprite;
        public Sprite crewmateSprite;
        public Sprite randomSprite;
        private int nodeType;
        private int randomNode;

        public void Awake() {
            SetNode();  // Set the type of event
            DetermineSprite(nodeType); // Determines the sprite displayed
            if (nodeType == 1) { // if the node is a random event..
                SetRandomEvent();   // set the type of event that will occur (random, battle, crewmate)
            }
            if (nodeType == 0) {    // if the node is a battle..
                CardGameManager.encounterDifficulty = Depth;    // pass the depth to the CardGameManager for the difficulty
            }
            Debug.Log($"{Depth}");
        }

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
                SceneManager.LoadScene("CrewmateScene");
            } else if (nodeType == 0) {
                SceneManager.LoadScene("BattleScene");
            } else {
                SceneManager.LoadScene("BattleScene");
            }
        }
    }
}