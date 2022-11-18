using System;
using Card;
using UnityEngine;

public class CardGameManager : MonoBehaviour {
	public static CardGameManager instance;
	public void Awake() => instance = this;

	
	public float turnTime = 30;

	public bool IsPlayerTurn => isPlayerTurn;
	public float TimeLeftInTurn => turnTimer;

	public Confirmation confirmationPrefab;
	
	private bool isPlayerTurn = true;
	private float turnTimer = 0;
	public void Update() {
		turnTimer -= Time.deltaTime;
		
		// If there is no longer any time left in the turn, end the turn!
		if (turnTimer <= 0) {
			turnTimer = turnTime;
			OnTurnEnd();
		}
	}

	public void OnTurnStart() {
		
	}

	public void OnTurnEnd() {
		// Toggle who's turn it is;
		isPlayerTurn = !isPlayerTurn;
		
		OnTurnStart();
	}
	

	public Confirmation CreateSnapConfirmation(CardBase card, CardContainerBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.snapTarget = target;
		return confirm;
	}
	
	public Confirmation CreateTargetConfirmation(CardBase card, CardBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.target = target;
		return confirm;
	}
}
