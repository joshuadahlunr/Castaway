using System;
using Card;
using UnityEngine;

public class CardGameManager : MonoBehaviour {
	public static CardGameManager instance;
	public void Awake() => instance = this;

	public Canvas canvas;
	public BurningRope rope;

	
	public float turnTime = 30;
	public int playerHealth = 10;

	public Deck playerDeck, playerGraveyard;
	public Hand playerHand;

	public bool IsPlayerTurn => isPlayerTurn;
	public float TimeLeftInTurn => turnTimer;

	public Confirmation confirmationPrefab;
	
	private bool isPlayerTurn = true;
	private float turnTimer = 0;

	public void Start() => OnTurnStart();

	public void Update() {
		turnTimer -= Time.deltaTime;

		rope.max = turnTime;
		rope.current = turnTimer;
		
		// If there is no longer any time left in the turn, end the turn!
		if (turnTimer <= 0) {
			turnTimer = turnTime;
			OnTurnEnd();
		}
	}

	public void OnTurnStart() {
		if (isPlayerTurn) {
			var missingCards = Math.Max(5 - playerHand.Count, 0);
			for (var i = 0; i < missingCards; i++) {
				if (playerDeck.Count <= 1) {
					// Shuffle the graveyard into the deck
					playerGraveyard.SendAllToContainer(playerDeck);
					playerDeck.Shuffle();
				}
				
				playerDeck.SendToContainer(0, playerHand);
			}
		} else {
			// TODO: Implement monster side!
		}
		
	}

	public void OnTurnEnd() {
		// Toggle who's turn it is;
		isPlayerTurn = !isPlayerTurn;
		
		// TODO: Check if the player's health is zero or if all of the monster's healths are zero!
		
		OnTurnStart();
	}
	

	public Confirmation CreateSnapConfirmation(CardBase card, CardContainerBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.snapTarget = target;
		return confirm;
	}
	
	public Confirmation CreateTargetConfirmation(CardBase card, CardBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.target = target;
		return confirm;
	}
}
