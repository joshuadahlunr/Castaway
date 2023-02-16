using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

/// <summary>
/// @author: Jared White
/// Swaps the scene depending on numbers generated randomly with probability
/// </summary>

public class SwapScene : MonoBehaviour {

    public virtual void SetScene() {
        // Sets the scene from the values given from DetermineNode()
        int eventType =  DetermineNode(); 
        if(eventType == 1) {
            // Random
            SceneManager.LoadScene("RandomScene");
        } else if(eventType == 2){
            // Crewmate
            SceneManager.LoadScene("CrewmateScene");
        } else if(eventType == 0){
            // Battle
            SceneManager.LoadScene("BattleScene");
        } else {
            SceneManager.LoadScene("BattleScene");
        }   
    }

	/*public void SpawnNode() {
		int type = SetNode();
		switch (type) {
			case 1:
				Instantiate(RandomEvent, transform.position, transform.rotation);
				break;
			case 2:
				Instantiate(BattleEvent, transform.position, transform.rotation);
				break;
			case 3:
				Instantiate(CrewmateEvent, transform.position, transform.rotation);
				break;
			default:
				Instantiate(BattleEvent, transform.position, transform.rotation);
				break;
		}
	}*/

    public int DetermineNode() {
		// Determines the type of node/event
		// Spawn with probability
		Random ranNode = new Random();
		int nodeValue = ranNode.Next(0,10);
        // If the value is 0-5, the event node type is Battle
        // 60% spawn rate
        if  (nodeValue <= 5) {
            Debug.Log("Battle");
            return 0; 
        } else if (nodeValue > 5 || nodeValue <= 8){
        // If the value is 5-7, the event node type is Crewmate
        // 30% spawn rate (can occur as a random event)
            Debug.Log("Random");
            return 1; 
        } else if (nodeValue > 8 || nodeValue <= 9){
        // If the value is 8-9 the event node type is Battle
        // 10% spawn rate
            Debug.Log("Crewmate");
            return 2; 
        } else {
            Debug.Log("Battle");
            return 0;
        }
    }
}