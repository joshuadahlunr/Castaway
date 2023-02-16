using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DetermineNode : MonoBehaviour {

	public GameObject BattleEvent;
	public GameObject RandomEvent;
	public GameObject CrewmateEvent;

	public void Awake(){
		// Is called 
		SpawnNode();
	}
	
	public void SpawnNode() {
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
	}

	public int SetNode() {
		// Determines the type of node/event
		// Spawn with probability
		Random ranNode = new Random();
		int nodeValue = ranNode.Next(0,10);
        if  (nodeValue <= 2) {
            Console.WriteLine("Random");
            return 1;
        } else if (nodeValue > 2 || nodeValue <= 3){
        // If the value is 3, the event node type is Crewmate
        // 10% spawn rate (can occur as a random event)
            Console.WriteLine("Crewmate");
            return 2;
        } else if (nodeValue > 3 || nodeValue <= 9){
        // If the value is 4, 5, 6, 7, 8, 9, the event node type is Battle
        // 60% spawn rate
            Console.WriteLine("Battle");
            return 3;
        } else {
            return 3;
        }
	}
}