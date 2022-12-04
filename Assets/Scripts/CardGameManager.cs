using System;
using System.Linq;
using Card;
using Extensions;
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

	public void Start() {
		// If the player's deck hasn't been defined yet, create a deck which is just a bunch of prototype attacks
		if (!DatabaseManager.GetOrCreateTable<Deck.DeckList>().Any()) 
			DatabaseManager.database.InsertOrReplace(new Deck.DeckList() {
				name = "Player Deck",
				Cards = new[] { "Prototype AttackCard" }.Replicate(10).ToArray()
			});
		
		playerDeck.DatabaseLoad();
	}
	

	// Update will immediately reset the counter invoking the start of a new turn
	private bool isPlayerTurn = false; // Since the turn owner immediately flips, start as monster's turn so that the player's turn will be next
	private float turnTimer = 0;
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
		// Invoke the turn start event on all cards
		foreach(var card in Card.CardBase.ActiveCards)
			card.OnTurnStart();
		
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
		// Invoke the turn end event on all cards
		foreach(var card in Card.CardBase.ActiveCards)
			card.OnTurnEnd();
		
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
