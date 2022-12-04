using System;
using System.Linq;
using Card;
using Extensions;
using UnityEngine;

public class CardGameManager : MonoBehaviour {
	public static CardGameManager instance;

	public Canvas canvas;
	public BurningRope rope;
	
	// TODO: Improve
	public BurningRope healthBar;

	
	public float turnTime = 30;
	public int playerMaxHandSize = 5;
	public int playerHealth = 10;

	public Deck playerDeck, playerGraveyard;
	public Hand playerHand;

	public MonsterCardBase[] monsters;

	public bool IsPlayerTurn => isPlayerTurn;
	public float TimeLeftInTurn => turnTimer;

	public Confirmation confirmationPrefab;

	public void Awake() {
		// Setup singleton
		instance = this;
		
		// If the player's deck hasn't been defined yet, create a deck which is just a bunch of prototype attacks
		if (!DatabaseManager.GetOrCreateTable<Deck.DeckList>().Any()) {
			DatabaseManager.database.InsertOrReplace(new Deck.DeckList() {
				name = "Player Deck",
				Cards = new[] { "Prototype AttackCard" }.Replicate(10).ToArray()
			});
			DatabaseManager.database.InsertOrReplace(new Deck.DeckList() {
				name = "Shark Deck",
				Cards = new[] { "Prototype AttackCard" }.Replicate(10).ToArray()
			});
		}

		// Load decks from SQL and assign the monster's cards to the appropriate monster
		playerDeck.DatabaseLoad();
		for (var i = 0; i < monsters.Length; i++){
			var monster = monsters[i];
			monster.deck.DatabaseLoad("Shark Deck");
			monster.deck.AssignOwnerToCards(i);
		}
	}
	

	// Update will immediately reset the counter invoking the start of a new turn
	private bool isPlayerTurn = false; // Since the turn owner immediately flips, start as monster's turn so that the player's turn will be next
	private float turnTimer = 0;
	public void Update() {
		turnTimer -= Time.deltaTime;

		rope.max = turnTime;
		rope.current = turnTimer;

		healthBar.max = 10;
		healthBar.current = playerHealth;
		
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
			var missingCards = Math.Max(playerMaxHandSize - playerHand.Count, 0);
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
			foreach(var monster in monsters)
				if(monster?.isActiveAndEnabled ?? false)
					monster.deck.RevealCard();
		}
	}

	public void OnTurnEnd() {
		// Invoke the turn end event on all cards
		foreach(var card in Card.CardBase.ActiveCards)
			card.OnTurnEnd();

		if (!isPlayerTurn) {
			foreach(var monster in monsters)
				if(monster?.isActiveAndEnabled ?? false)
					monster.deck.PlayRevealedCard();
		}

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
