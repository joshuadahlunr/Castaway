using System;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;
using CardBattle.Containers;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CardBattle {
	/// <summary>
/// Singleton Manager responsible for controlling the flow of the card game
/// </summary>
/// <author>Joshua Dahl</author>
public class CardGameManager : MonoBehaviour {
	/// <summary>
	/// Singleton instance of this class
	/// </summary>
	public static CardGameManager instance;

	/// <summary>
	/// Variable set by the encounter map, used to determine how difficult this encounter should be!
	/// </summary>
	public static float encounterDifficulty = 1;

	/// <summary>
	/// The type of encounter this is (normal or boss)
	/// </summary>
	public enum EncounterType {
		Normal,
		Boss,
		FinalBoss
	}
	public static EncounterType encounterType = EncounterType.Normal;

	/// <summary>
	/// Reference to the main scene canvas
	/// </summary>
	public Canvas canvas;
	/// <summary>
	/// Reference to the timer countdown rope
	/// </summary>
	public BurningRope rope;

	public TMPro.TMP_Text health;

	public TMPro.TMP_Text PeopleJuiceText;

	// TODO: Improve
	public GameObject losePanel;

	/// <summary>
	/// Reference to the prefab for confirmation prompts
	/// </summary>
	public Confirmation confirmationPrefab;

	public MonsterDatabase monsterDatabase, bossDatabase, finalBossDatabase;


	/// <summary>
	/// The amount of time in a turn
	/// </summary>
	public float turnTime = 30;
	/// <summary>
	/// The number of cards the player's hand is refilled to every turn
	/// </summary>
	public int playerMaxHandSize = 5;

	/// <summary>
	/// Backing memory for the player's health
	/// </summary>
	[SerializeField] private HealthState _playerHealthState = new HealthState {health = 10};
	/// <summary>
	/// Players health (invokes a callback whenever changed)
	/// </summary>
	public HealthState playerHealthState {
		set {
			OnPlayerHealthStateChange(_playerHealthState, value);
			_playerHealthState = value;
		}
		get => _playerHealthState;
	}

	/// <summary>
	/// Variables tracking the player's people juice (both available and current)
	/// </summary>
	public PeopleJuice.Cost resetPeopleJuice, currentPeopleJuice;

	// References to the player's deck, graveyard, hand, containers, and ship
	public Deck playerDeck, playerGraveyard;
	public Hand playerHand;
	public CardContainerBase[] inPlayContainers;
	public GameObject ship;

	/// <summary>
	/// References to all of the monsters
	/// </summary>
	public Card.MonsterCardBase[] monsters;

	// Events that cards (and other things) can subscribe to
	public UnityEvent turnStart, turnEnd;
	public UnityEvent<HealthState, HealthState> playerHealthStateChange;

	/// <summary>
	/// Provide public read only access to who's turn it is
	/// </summary>
	public bool IsPlayerTurn => isPlayerTurn;
	/// <summary>
	/// Provide public read only access to how much time is left in the turn
	/// </summary>
	public float TimeLeftInTurn => turnTimer;


	/// <summary>
	/// When the game starts...
	/// </summary>
	public void Awake() {
		// Setup singleton
		instance = this;

		const string playerDeckName = "Player Deck";
		const string sharkDeckName = "Shark Deck";

		// If the player's deck hasn't been defined yet, create a deck which is just a bunch of prototype attacks
		if (!DatabaseManager.GetOrCreateTable<Deck.DeckList>().Any()) {
			Debug.Log("Creating decklist table");
			DatabaseManager.database.Insert(new Deck.DeckList() {
				name = playerDeckName,
			});
			DatabaseManager.database.Insert(new Deck.DeckList() {
				name = sharkDeckName,
			});
		}

		if (!DatabaseManager.GetOrCreateTable<Deck.DeckListCard>().Any()) {
			var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
				.FirstOrDefault(l => l.name == playerDeckName).id;
			var sharkDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
				.FirstOrDefault(l => l.name == sharkDeckName).id;

			Debug.Log("Creating decklist cards table");
			for (var i = 0; i < 10; i++) {
				DatabaseManager.database.Insert(new Deck.DeckListCard() {
					listID = playerDeckId,
					name = "Attack",
					level = 1,
				});

				DatabaseManager.database.Insert(new Deck.DeckListCard() {
					listID = sharkDeckId,
					name = "Attack",
					level = 2,
				});
			}
		}

		// Spawn the encounter
		for (var i = 0; i < 1; i++) { // TODO: spawn more monsters based on the difficulty
			var monster = encounterType switch {
				EncounterType.Normal => monsterDatabase.Instantiate(monsterDatabase.cards.Keys.Shuffle().First()),
				EncounterType.Boss => bossDatabase.Instantiate(bossDatabase.cards.Keys.Shuffle().First()),
				EncounterType.FinalBoss =>
					finalBossDatabase.Instantiate(finalBossDatabase.cards.Keys.Shuffle().First()),
				_ => throw new ArgumentOutOfRangeException()
			};
			monster.transform.localScale *= 2;
			// TODO: Position the monster in a circle around the ship
			monster.transform.position = new Vector3(-0.0300000049f, 0.556999981f, 0.141000032f);
			// TODO: Add a level modifier based on the difficulty
			monsters = new List<MonsterCardBase>(monsters) { monster }.ToArray();
		}

		// Load decks from SQL and assign the monster's cards to the appropriate monster
		playerDeck.DatabaseLoad(playerDeckName);
		for (var i = 0; i < monsters.Length; i++){
			var m = monsters[i];
			// m.deck.DatabaseLoad("Shark Deck");
			m.deck.AssignOwnerToCards(i);
		}

		// Invoke the turn start event
		turnStart?.Invoke();
	}



	/// <summary>
	/// Variable tracking if it is the player's turn or not
	/// </summary>
	private bool isPlayerTurn = false; // Since the turn owner immediately flips, start as monster's turn so that the player's turn will be next
	/// <summary>
	/// Variable tracking how much time remains in the turn
	/// </summary>
	private float turnTimer = 0;
	/// <summary>
	/// Every frame...
	/// </summary>
	public void Update() {
		// Decrease the time left in the turn
		turnTimer -= Time.deltaTime;
		rope.max = turnTime;
		rope.current = turnTimer;

		// Update the player's health
		health.text = "" + playerHealthState.health;

		PeopleJuiceText.text = currentPeopleJuice.ToString();

		// If there is no longer any time left in the turn, end the turn!
		if (turnTimer <= 0) {
			turnTimer = turnTime;
			OnTurnEnd();
		}
	}

	/// <summary>
	/// Called whenever a new turn begins
	/// </summary>
	public void OnTurnStart() {
		// Invoke the turn start event on all cards
		foreach(var card in Card.CardBase.ActiveCards)
			card.OnTurnStart();

		if (isPlayerTurn) {
			// Reset their damage negation
			playerHealthState = playerHealthState.SetTemporaryDamageReduction(0);

			// Refill the player's people juice
			currentPeopleJuice = new PeopleJuice.Cost(resetPeopleJuice);
			// Enable all of the cards in their hand that were disabled
			EnableCards(CardFilterer.EnumerateAllCards()); // TODO: Should we be more specific with which cards are renabled?

			// Refill the player's hand
			var missingCards = Math.Max(playerMaxHandSize - playerHand.Count, 0);
			for (var i = 0; i < missingCards; i++)
				DrawPlayerCard();
		} else
			foreach(var monster in monsters)
				if ( !(monster?.Disabled ?? true) ) {
					monster.healthState = monster.healthState.SetTemporaryDamageReduction(0); // Reset their damage negation
					monster.deck.RevealCard();
				}

		// Disable all of the unaffordable cards in the player's hand!
		OnlyEnableAffordableCards();
		turnStart?.Invoke();
	}

	/// <summary>
	/// Called whenever a turn ends
	/// </summary>
	public void OnTurnEnd() {
		// Invoke the turn end event on all cards
		foreach(var card in Card.CardBase.ActiveCards)
			card.OnTurnEnd();

		if (!isPlayerTurn) {
			foreach(var monster in monsters)
				if( !(monster?.Disabled ?? true) )
					monster.deck.PlayRevealedCard();
		}

		// Toggle who's turn it is;
		isPlayerTurn = !isPlayerTurn;

		CheckWinLose();
		turnEnd?.Invoke();

		// TODO: Show a turn transition screen here!
		OnTurnStart();
	}

	/// <summary>
	/// Callback called whenever the player's health changes
	/// </summary>
	/// <param name="oldHealth">The player's old health</param>
	/// <param name="newHealth">The player's current health</param>
	public void OnPlayerHealthStateChange(HealthState oldHealth, HealthState newHealth) {
		if(oldHealth != newHealth)
			Debug.Log($"Player took {oldHealth - newHealth} damage");
		CheckWinLose();
		playerHealthStateChange?.Invoke(oldHealth, newHealth);
	}

	/// <summary>
	/// Callback called when the player wins the game
	/// </summary>
	public void OnWin() {
		SceneManager.LoadScene("Scenes/ResourceMgmtScene");
	}

	/// <summary>
	/// Callback called when the player loses the game
	/// </summary>
	public void OnLose() {
		Time.timeScale = 0; // Pause
		losePanel.SetActive(true); // Display the lose panel
		// TODO: Need to go back to the main menu or something...
	}


	/// <summary>
	/// Function which draws a card for the player (shuffling the graveyard into their deck if necessary)
	/// </summary>
	public void DrawPlayerCard() {
		// If drawing the card would leave the deck empty...
		if (playerDeck.Count <= 1) {
			// Shuffle the graveyard into the deck
			playerGraveyard.SendAllToContainer(playerDeck);
			playerDeck.Shuffle();
		}

		// Send a card from the player's deck to their hand
		if(playerDeck.Count > 0)
			playerDeck.SendToContainer(0, playerHand);
	}

	/// <summary>
	/// Check if the player has won (all monsters defeated) or lost (has 0 HP left) and invoke the appropriate events
	/// </summary>
	public void CheckWinLose() {
		if (playerHealthState.health <= 0)
			OnLose();

		bool allDead = monsters.All(monster => monster.Disabled);
		if(allDead)
			OnWin();
	}



	/// <summary>
	/// Function which disables all of the given cards
	/// </summary>
	/// <param name="cards">The cards to disable</param>
	public static void DisableCards(IEnumerable<Card.CardBase> cards) {
		foreach (var card in cards)
			card.MarkDisabled();
	}

	/// <summary>
	/// Function which enables all of the given cards
	/// </summary>
	/// <param name="cards">The cards to enable</param>
	public static void EnableCards(IEnumerable<Card.CardBase> cards) {
		foreach (var card in cards)
			card.MarkEnabled();
	}

	/// <summary>
	/// Function which enables all of the affordable cards in the player's hand, and disables all of the unaffordable cards in their hand!
	/// </summary>
	public void OnlyEnableAffordableCards() {
		// Enable all of the affordable cards in hand!
		EnableCards(CardFilterer.FilterCards(~(CardFilterer.CardFilters.Affordable | CardFilterer.CardFilters.Hand | CardFilterer.CardFilters.Player | CardFilterer.CardFilters.Status | CardFilterer.CardFilters.Action)));
		// Disable all of the unaffordable cards
		CardFilterer.FilterAndDisableCards(CardFilterer.CardFilters.Unaffordable);
	}




	/// <summary>
	/// Function which creates a basic attack card (invoked when the player bins a card)
	/// </summary>
	/// <returns>Reference to the newly created attack card</returns>
	public Card.CardBase InstantiateBinnedAttack() {
		return playerDeck.cardDB.Instantiate("Binned Attack");
	}

        /// <summary>
        /// Lock which prevents the player from creating multiple card confirmations...
        /// </summary>
        public bool activeConfirmationExists;

	/// <summary>
	/// Creates a snap confirmation
	/// </summary>
	/// <param name="card">The card to snap into place if confirmed</param>
	/// <param name="target">The container to move the card to if confirmed</param>
	/// <returns>Reference to the created confirmation</returns>
	public Confirmation CreateSnapConfirmation(Card.CardBase card, CardContainerBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.snapTarget = target;
		if(activeConfirmationExists){ // Don't let the player play a new card if they are sill confirming one...
			confirm.Cancel();
			return null;
		}
		activeConfirmationExists = true;
		return confirm;
	}

	/// <summary>
	/// Creates a target confirmation
	/// </summary>
	/// <param name="card">The card that is performing the targeting</param>
	/// <param name="target">The card this is being targeted</param>
	/// <returns>Reference to the created confirmation</returns>
	public Confirmation CreateTargetConfirmation(Card.CardBase card, Card.CardBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.target = target;
		if(activeConfirmationExists){ // Don't let the player play a new card if they are sill confirming one...
			confirm.Cancel();
			return null;
		}
		activeConfirmationExists = true;

		if (card is not Card.ActionCardBase aCard) return confirm;
		if (PeopleJuice.CostAvailable(currentPeopleJuice, aCard.cost)) return confirm;
		confirm.Cancel();
		return null;
	}

	/// <summary>
	/// Creates a bin confirmation
	/// </summary>
	/// <param name="card">The card that may be binned</param>
	/// <param name="bin">The graveyard to send the binned card to</param>
	/// <returns>Reference to the created confirmation</returns>
	public Confirmation CreateBinConfirmation(Card.CardBase card, Graveyard bin) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.bin = bin;
		return confirm;
	}
}
}
