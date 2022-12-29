using System;
using System.Linq;
using Card;
using CardBattle;
using Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	/// Reference to the main scene canvas
	/// </summary>
	public Canvas canvas;
	/// <summary>
	/// Reference to the timer countdown rope
	/// </summary>
	public BurningRope rope;
	
	// TODO: Improve
	public BurningRope healthBar;
	public GameObject losePanel;
	
	/// <summary>
	/// Reference to the prefab for confirmation prompts
	/// </summary>
	public Confirmation confirmationPrefab;

	
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
	[SerializeField] private int _playerHealth = 10;
	/// <summary>
	/// Players health (invokes a callback whenever changed)
	/// </summary>
	public int playerHealth {
		set {
			OnPlayerHealthChange(_playerHealth, value);
			_playerHealth = value;
		}
		get => _playerHealth;
	}

	// References to the player's deck, graveyard, and hand
	public Deck playerDeck, playerGraveyard;
	public Hand playerHand;

	/// <summary>
	/// References to all of the monsters
	/// </summary>
	public MonsterCardBase[] monsters;

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
		
		// If the player's deck hasn't been defined yet, create a deck which is just a bunch of prototype attacks
		if (!DatabaseManager.GetOrCreateTable<Deck.DeckList>().Any()) {
			DatabaseManager.database.InsertOrReplace(new Deck.DeckList() {
				name = "Player Deck",
				Cards = new[] { "Prototype AttackCard" }.Replicate(6).ToArray()
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

		PeopleJuice.Cost pool = new() { PeopleJuice.Types.B, PeopleJuice.Types.Generic, PeopleJuice.Types.A, PeopleJuice.Types.Generic, PeopleJuice.Types.B };
		PeopleJuice.Cost cost = new() { PeopleJuice.Types.Generic, PeopleJuice.Types.Generic, PeopleJuice.Types.B};

		PeopleJuice.DeductCost(ref pool, cost);
		Debug.Log(pool);	
		// Debug.Log(PeopleJuice.CostAvailable(pool, cost));
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

		// Update the player's healthbar
		healthBar.max = 10;
		healthBar.current = playerHealth;
		
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
			var missingCards = Math.Max(playerMaxHandSize - playerHand.Count, 0);
			for (var i = 0; i < missingCards; i++)
				DrawPlayerCard();
		} else {
			// TODO: Implement monster side!
			foreach(var monster in monsters)
				if(monster?.isActiveAndEnabled ?? false)
					monster.deck.RevealCard();
		}
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
				if(monster?.isActiveAndEnabled ?? false)
					monster.deck.PlayRevealedCard();
		}

		// Toggle who's turn it is;
		isPlayerTurn = !isPlayerTurn;

		CheckWinLose();
		
		OnTurnStart();
	}

	/// <summary>
	/// Callback called whenever the player's health changes
	/// </summary>
	/// <param name="oldHealth">The player's old health</param>
	/// <param name="newHealth">The player's current health</param>
	public void OnPlayerHealthChange(int oldHealth, int newHealth) {
		CheckWinLose();
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
		playerDeck.SendToContainer(0, playerHand);
	} 
	
	/// <summary>
	/// Check if the player has won (all monsters defeated) or lost (has 0 HP left) and invoke the appropriate events
	/// </summary>
	public void CheckWinLose() {
		if (playerHealth <= 0)
			OnLose();

		bool allDead = monsters.All(monster => !monster.isActiveAndEnabled);
		if(allDead)
			OnWin();
	}

	/// <summary>
	/// Creates a snap confirmation
	/// </summary>
	/// <param name="card">The card to snap into place if confirmed</param>
	/// <param name="target">The container to move the card to if confirmed</param>
	/// <returns>Reference to the created confirmation</returns>
	public Confirmation CreateSnapConfirmation(CardBase card, CardContainerBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.snapTarget = target;
		return confirm;
	}
	
	/// <summary>
	/// Creates a target confirmation
	/// </summary>
	/// <param name="card">The card that is performing the targeting</param>
	/// <param name="target">The card this is being targeted</param>
	/// <returns>Reference to the created confirmation</returns>
	public Confirmation CreateTargetConfirmation(CardBase card, CardBase target) {
		var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
		confirm.card = card;
		confirm.target = target;
		return confirm;
	}
}
