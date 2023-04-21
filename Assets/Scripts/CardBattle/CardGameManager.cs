using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;
using CardBattle.Containers;
using Crew;
using Extensions;
using ResourceMgmt;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace CardBattle {
	/// <summary>
	///     Singleton Manager responsible for controlling the flow of the card game
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class CardGameManager : MonoBehaviour {
		/// <summary>
		///     Singleton instance of this class
		/// </summary>
		public static CardGameManager instance;

		/// <summary>
		///     Variable set by the encounter map, used to determine how difficult this encounter should be!
		/// </summary>
		public static float encounterDifficulty;

		/// <summary>
		///     The type of encounter this is (normal or boss)
		/// </summary>
		public enum EncounterType {
			Normal,
			Boss,
			FinalBoss
		}
		public static EncounterType encounterType = EncounterType.Normal;

		public static int numberOfMonstersKilled;
		public static int monsterLevel;

		/// <summary>
		///     Reference to the main scene canvas
		/// </summary>
		public Canvas canvas;

		/// <summary>
		///     Reference to the timer countdown rope
		/// </summary>
		public BurningRope rope;

		/// <summary>
		///		Reference to the post processing volume which makes the game look like an old film
		/// </summary>
		public Volume OldFilmPostProcessing;

		/// <summary>
		///	Ship sprites that get swapped out every 5 levels
		/// </summary>
		public Material material0, material5, material10;

		/// <summary>
		///		Equipment slots that get unlocked every 3 levels
		/// </summary>
		public GameObject slot3, slot6, slot9;

		/// <summary>
		///     Text field showing the player's health
		/// </summary>
		public TMP_Text health;

		/// <summary>
		///     FlowLayoutGroup used to hold People Juice icons
		/// </summary>
		public FlowLayoutGroup PeopleJuiceHolder;

		/// <summary>
		///     Database of People Juice icons
		/// </summary>
		public PeopleJuice peopleJuiceIconDatabase;

		/// <summary>
		///     Prefab for a People Juice icon
		/// </summary>
		public Image PeopleJuiceIconPrefab;

		/// <summary>
		///     Reference to the prefab for confirmation prompts
		/// </summary>
		public Confirmation confirmationPrefab;

		public GameObject playerTurnPrefab, monsterTurnPrefab, playerWinPrefab;

		/// <summary>
		///     Database of all monsters in the game
		/// </summary>
		public MonsterDatabase monsterDatabase, bossDatabase, finalBossDatabase;

		/// <summary>
		///     The amount of time in a turn
		/// </summary>
		public float playerTurnTime = 30;
		public float monsterTurnTime = 5;

		/// <summary>
		///     The maximum number of cards in the player's hand
		/// </summary>
		public int playerMaxHandSize = 5;

		/// <summary>
		///     Backing memory for the player's health state
		/// </summary>
		[SerializeField] private HealthState _playerHealthState = new() { health = 10 };

		/// <summary>
		///     Player's health state, with callback when changed
		/// </summary>
		public HealthState playerHealthState {
			set {
				OnPlayerHealthStateChange(_playerHealthState, value);
				_playerHealthState = value;
			}
			get => _playerHealthState;
		}

		/// <summary>
		///     Variables tracking the player's People Juice (both available and current)
		/// </summary>
		public PeopleJuice.Cost resetPeopleJuice, currentPeopleJuice;

		/// <summary>
		/// Variable which when set causes people juice to not get loaded from the database!
		/// </summary>
		public bool debugPeopleJuice = false;

		// References to the player's deck, graveyard, hand, containers, and ship
		public Deck playerDeck, playerGraveyard;
		public Hand playerHand;
		public CardContainerBase[] inPlayContainers;
		public MeshRenderer ship;
		public GameObject ocean;

		/// <summary>
		/// Bool that can disable monsters for testing purposes
		/// </summary>
		public string spawnSpecificMonster = "";
		/// <summary>
		///     References to all of the monsters
		/// </summary>
		public MonsterCardBase[] monsters;

		// Events that cards (and other things) can subscribe to
		public UnityEvent turnStart, turnEnd;
		public UnityEvent<HealthState, HealthState> playerHealthStateChange;

		/// <summary>
		///     Provide public read only access to who's turn it is
		/// </summary>
		public bool IsPlayerTurn => isPlayerTurn;

		/// <summary>
		///     Provide public read only access to how much time is left in the turn
		/// </summary>
		public float TimeLeftInTurn => turnTimer;


		/// <summary>
		///     This function is called when the game starts. It sets up the game by creating decks for the player and the shark, creating monsters based on the current encounter difficulty, and assigning cards to each monster. It also invokes the turn start event to start the game.
		/// </summary>
		public void Awake() {
			// Setup singleton
			instance = this;

			// Define some constants for the names of the player's deck and the shark's deck
			const string playerDeckName = "Player Deck";
			const string sharkDeckName = "Shark Deck";

			// If the player's deck hasn't been defined yet, create a deck which is just a bunch of prototype attacks
			if (!DatabaseManager.GetOrCreateTable<Deck.DeckList>().Any()) {
				Debug.Log("Creating decklist table");
				DatabaseManager.database.Insert(new Deck.DeckList {
					name = playerDeckName
				});
				DatabaseManager.database.Insert(new Deck.DeckList {
					name = sharkDeckName
				});
			}

			// If the player's deck doesn't have any cards in it yet, add 10 "Attack" cards to the player's deck and 10 "Attack" cards to the shark's deck
			var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
				.FirstOrDefault(l => l.name == playerDeckName).id;
			if (!DatabaseManager.GetOrCreateTable<Deck.DeckListCard>().Any()) {
				var sharkDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
					.FirstOrDefault(l => l.name == sharkDeckName).id;

				Debug.Log("Creating decklist cards table");
				for (var i = 0; i < 2; i++) {
					DatabaseManager.database.Insert(new Deck.DeckListCard {
						listID = playerDeckId,
						name = "Attack",
						level = 2,
						associatedCrewmateID = null
					});
					// DatabaseManager.database.Insert(new Deck.DeckListCard {
					// 	listID = playerDeckId,
					// 	name = "Swarm Block",
					// 	level = 2,
					// 	associatedCrewmateID = null
					// });
					// DatabaseManager.database.Insert(new Deck.DeckListCard {
					// 	listID = playerDeckId,
					// 	name = "Damaged Harpoon",
					// 	level = 2,
					// 	associatedCrewmateID = null
					// });
					DatabaseManager.database.Insert(new Deck.DeckListCard {
						listID = playerDeckId,
						name = "Bail Water",
						level = 2,
						associatedCrewmateID = null
					});
					DatabaseManager.database.Insert(new Deck.DeckListCard {
						listID = playerDeckId,
						name = "Jerry's Arcane Twirl",
						level = 2,
						associatedCrewmateID = null
					});
				}
			}

			// Calculate the ship level
			var shipUpgradeInfo = DatabaseManager.GetOrCreateTable<ResourceManager.UpgradeInfo>().FirstOrDefault();
			var shipLevel = shipUpgradeInfo?.currentLvl ?? 0; // TODO: Spawn proper ship upgrade using this
			shipLevel = 3;

			// Set the material of the ship to track its upgrades
			ship.material = material0;
			if (shipLevel >= 5) ship.material = material5;
			if (shipLevel >= 10) ship.material = material10;

			// Set the number of slots based on the ship level
			slot3.SetActive(shipLevel >= 3);
			slot6.SetActive(shipLevel >= 6);
			slot9.SetActive(shipLevel >= 10);

			// Update Jerry's cards to have the same level as the ship
			var playerCards = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
				.Where(card => card.listID == playerDeckId);
			foreach (var card in playerCards)
				if (card.associatedCrewmateID == null) {
					card.level = shipLevel;
					DatabaseManager.database.InsertOrReplace(card);
				}

			// Set reset people juice to match the current crewmates
			if (!debugPeopleJuice) {
				var reset = new PeopleJuice.Cost { PeopleJuice.Types.Wizard }; // Jerry gives a wizard

				var crewmates = DatabaseManager.GetOrCreateTable<CrewManager.CrewData>();
				foreach (var crewmate in crewmates)
					reset.Add((PeopleJuice.Types)crewmate.type);

				// Update all the juices to match
				currentPeopleJuice = resetPeopleJuice = reset;
			}

			// Determine the level of the encounter based on the difficulty, and if it's a multiple of 5 - 1, set the encounter type to "Boss"
			monsterLevel = (int)Mathf.Max(Mathf.Round(encounterDifficulty), 0) + 1;
			if (monsterLevel % 5 == 4)
				encounterType = EncounterType.Boss;
			if (monsterLevel > 20)
				encounterType = EncounterType.FinalBoss;


			// Calculate the number of encounters to spawn based on the encounter type and level
			numberOfMonstersKilled = encounterType == EncounterType.Normal ? monsterLevel / 5 + 1 : 1;
			// Spawn the encounters
			for (var i = 0; i < numberOfMonstersKilled; i++) {
				MonsterCardBase monster;
				if (string.IsNullOrEmpty(spawnSpecificMonster)) {
					monster = encounterType switch {
						EncounterType.Normal => monsterDatabase.Instantiate(
							monsterDatabase.cards.Keys.Shuffle().First()),
						EncounterType.Boss => bossDatabase.Instantiate(bossDatabase.cards.Keys.Shuffle().First()),
						EncounterType.FinalBoss => finalBossDatabase.Instantiate(finalBossDatabase.cards.Keys
							.Shuffle()
							.First()),
						_ => throw new ArgumentOutOfRangeException()
					};
				} else monster = (monsterDatabase.Instantiate(spawnSpecificMonster)
				                  ?? bossDatabase.Instantiate(spawnSpecificMonster))
				                  ?? finalBossDatabase.Instantiate(spawnSpecificMonster);

				monster.Position();

				// Add a modification to each card in the monster's deck to adjust its difficulty based on the level of the encounter
				foreach (var card in monster.deck) {
					Debug.Log($"Upgrading {card.name}");
					card.AddModification(new LevelModification(monsterLevel));
				}

				monster.AddModification(new LevelModification(monsterLevel));
				monsters = new List<MonsterCardBase>(monsters) { monster }.ToArray();
			}

			// Load the player's deck from SQL
			playerDeck.DatabaseLoad();
			// Assign the monster cards to their monster
			for (var i = 0; i < monsters.Length; i++) {
				var m = monsters[i];
				m.deck.AssignOwnerToCards(i);
			}

			// Invoke the turn start event
			turnStart?.Invoke();
		}

		public void Start() {
			AudioManager.instance.PlayBattleMusic();
		}


		/// <summary>
		///     Variable tracking if it is the player's turn or not
		/// </summary>
		private bool isPlayerTurn; // Since the turn owner immediately flips, start as monster's turn so that the player's turn will be next

		/// <summary>
		///     Variable tracking how much time remains in the turn
		/// </summary>
		private float turnTimer;

		/// <summary>
		///     Every frame...
		/// </summary>
		private PeopleJuice.Cost oldCost;
		private int disabledCounter = 0;
		public void Update() {
			// Decrease the time left in the turn
			turnTimer -= Time.deltaTime;
			// Update the rope UI element to reflect the remaining turn time
			rope.max = isPlayerTurn ? playerTurnTime : monsterTurnTime;
			rope.current = turnTimer;

			// Update the player's health UI element
			health.text = "" + playerHealthState.health;

			// Check if the "People Juice" UI elements need updating
			if (oldCost != currentPeopleJuice) {
				Debug.Log("People Juice Updated!");
				// Destroy existing icons and create new ones based on the current "People Juice" values
				foreach (Transform icon in PeopleJuiceHolder.transform)
					Destroy(icon.gameObject);
				foreach (var icon in currentPeopleJuice) {
					var uiElement = Instantiate(PeopleJuiceIconPrefab.gameObject, PeopleJuiceHolder.transform)
						.GetComponent<Image>();
					uiElement.sprite = peopleJuiceIconDatabase.sprites[icon];
				}

				// Update the layout of the "People Juice" UI element holder
				PeopleJuiceHolder.CalculateLayoutInputVertical();
			}

			// Store the current "People Juice" values for comparison in the next frame
			oldCost = currentPeopleJuice;

			// While all the cards in the player's hand are disabled increase disabled counter, if it is high for a few frames the player has no playable cards in hand!
			if (playerHand.All(card => card.Disabled) && isPlayerTurn)
				disabledCounter++;

			// If the turn timer has reached zero or the player has no playable cards remaining, end the turn
			if (turnTimer <= 0 || disabledCounter >= 60) {
				turnTimer = isPlayerTurn ? monsterTurnTime : playerTurnTime;
				OnTurnEnd();
			}
		}

		/// <summary>
		///     Called whenever a new turn begins
		/// </summary>
		public void OnTurnStart() {
			// Invoke the turn start event on all cards
			foreach (var card in CardBase.ActiveCards)
				card.OnTurnStart();

			disabledCounter = 0;

			if (isPlayerTurn) {
				OldFilmPostProcessing.enabled = false;
				Instantiate(playerTurnPrefab, canvas.transform);

				// Reset their damage negation
				playerHealthState = playerHealthState.SetTemporaryDamageReduction(0);

				// Refill the player's people juice
				currentPeopleJuice = new PeopleJuice.Cost(resetPeopleJuice);

				// Enable all of the cards in their hand that were disabled
				EnableCards(CardFilterer
					.EnumerateAllCards()); // TODO: Should we be more specific with which cards are renabled?

				// Refill the player's hand
				var missingCards = Math.Max(playerMaxHandSize - playerHand.Count, 0);
				for (var i = 0; i < missingCards; i++)
					DrawPlayerCard();
			} else {
				OldFilmPostProcessing.enabled = true;
				Instantiate(monsterTurnPrefab, canvas.transform);

				// If it's not the player's turn, reset the damage negation of all active monsters and reveal their top card
				foreach (var monster in monsters)
					if (!(monster?.Disabled ?? true)) {
						monster.healthState = monster.RawHealth.SetTemporaryDamageReduction(0);
						monster.deck.RevealCard();
					}
			}

			// Disable all of the unaffordable cards in the player's hand!
			OnlyEnableAffordableCards();

			// Invoke the turn start event
			turnStart?.Invoke();
		}

		/// <summary>
		///     Called whenever a turn ends
		/// </summary>
		public void OnTurnEnd() {
			// Invoke the turn end event on all cards
			foreach (var card in CardBase.ActiveCards)
				card.OnTurnEnd();

			if (!isPlayerTurn)
				// If it's not the player's turn, play the revealed card of all active monsters
				foreach (var monster in monsters)
					if (!(monster?.Disabled ?? true))
						monster.deck.PlayRevealedCard();

			// Toggle who's turn it is
			isPlayerTurn = !isPlayerTurn;

			// Check for win or lose conditions
			CheckWinLose();

			// Invoke the turn end event
			turnEnd?.Invoke();

			// Start the next turn
			OnTurnStart();
		}

		/// <summary>
		/// Helper method used as a callback to UI hover events
		/// </summary>
		public void PlayUISound(string sound = "Interact") => AudioManager.instance?.uiSoundFXPlayer?.PlayTrackImmediate(sound);

		/// <summary>
		///     Callback called whenever the player's health changes
		/// </summary>
		/// <param name="oldHealth">The player's old health</param>
		/// <param name="newHealth">The player's current health</param>
		public void OnPlayerHealthStateChange(HealthState oldHealth, HealthState newHealth) {
			if (oldHealth != newHealth)
				Debug.Log($"Player took {oldHealth - newHealth} damage");
			CheckWinLose();
			playerHealthStateChange?.Invoke(oldHealth, newHealth);
		}

		/// <summary>
		///     Callback called when the player wins the game
		/// </summary>
		public void OnWin() {
			IEnumerator WinCoroutine() {
				var winScreen = Instantiate(playerWinPrefab, canvas.transform);
				while (winScreen != null)
					yield return null;
				SceneManager.LoadScene("Scenes/ResourceMgmtScene");
			}

			StartCoroutine(WinCoroutine());
			CrewManager.XPGain();
		}

		/// <summary>
		///     Callback called when the player loses the game
		/// </summary>
		public void OnLose() => SceneManager.LoadScene("Scenes/LoseScene");


		/// <summary>
		///     Function which draws a card for the player (shuffling the graveyard into their deck if necessary)
		/// </summary>
		public void DrawPlayerCard() {
			// If drawing the card would leave the deck empty...
			if (playerDeck.Count <= 1) {
				// Shuffle the graveyard into the deck
				playerGraveyard.SendAllToContainer(playerDeck);
				playerDeck.Shuffle();
			}

			// Send a card from the player's deck to their hand
			if (playerDeck.Count > 0)
				playerDeck.SendToContainer(0, playerHand);
		}

		/// <summary>
		///     Check if the player has won (all monsters defeated) or lost (has 0 HP left) and invoke the appropriate events
		/// </summary>
		public void CheckWinLose() {
			if (playerHealthState.health <= 0)
				OnLose();

			var allDead = monsters.All(monster => monster.healthState.health <= 0);
			if (allDead)
				OnWin();
		}


		/// <summary>
		///     Function which disables all of the given cards
		/// </summary>
		/// <param name="cards">The cards to disable</param>
		public static void DisableCards(IEnumerable<CardBase> cards) {
			foreach (var card in cards)
				card.MarkDisabled();
		}

		/// <summary>
		///     Function which enables all of the given cards
		/// </summary>
		/// <param name="cards">The cards to enable</param>
		public static void EnableCards(IEnumerable<CardBase> cards) {
			foreach (var card in cards)
				card.MarkEnabled();
		}

		/// <summary>
		///     Function which enables all of the affordable cards in the player's hand, and disables all of the unaffordable cards in their hand!
		/// </summary>
		public void OnlyEnableAffordableCards() {
			// Enable all of the affordable cards in hand!
			EnableCards(CardFilterer.FilterCards(~(CardFilterer.CardFilters.Affordable | CardFilterer.CardFilters.Hand |
			                                       CardFilterer.CardFilters.Player | CardFilterer.CardFilters.Status |
			                                       CardFilterer.CardFilters.Action)));
			// Disable all of the unaffordable cards
			CardFilterer.FilterAndDisableCards(CardFilterer.CardFilters.Unaffordable);
		}


		/// <summary>
		///     Function which creates a basic attack card (invoked when the player bins a card)
		/// </summary>
		/// <returns>Reference to the newly created attack card</returns>
		public CardBase InstantiateBinnedAttack() => playerDeck.cardDB.Instantiate("Binned Attack");

		/// <summary>
		///     Lock which prevents the player from creating multiple card confirmations...
		/// </summary>
		public bool activeConfirmationExists;

		/// <summary>
		///     Creates a snap confirmation
		/// </summary>
		/// <param name="card">The card to snap into place if confirmed</param>
		/// <param name="target">The container to move the card to if confirmed</param>
		/// <returns>Reference to the created confirmation</returns>
		public Confirmation CreateSnapConfirmation(CardBase card, CardContainerBase target) {
			var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
			confirm.card = card;
			confirm.snapTarget = target;
			if (activeConfirmationExists) {
				// Don't let the player play a new card if they are sill confirming one...
				confirm.Cancel();
				return null;
			}

			activeConfirmationExists = true;
			return confirm;
		}

		/// <summary>
		///     Creates a target confirmation
		/// </summary>
		/// <param name="card">The card that is performing the targeting</param>
		/// <param name="target">The card this is being targeted</param>
		/// <returns>Reference to the created confirmation</returns>
		public Confirmation CreateTargetConfirmation(CardBase card, CardBase target) {
			var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
			confirm.card = card;
			confirm.target = target;
			if (activeConfirmationExists) {
				// Don't let the player play a new card if they are sill confirming one...
				confirm.Cancel();
				return null;
			}

			activeConfirmationExists = true;

			if (card is not ActionCardBase aCard) return confirm;
			if (PeopleJuice.CostAvailable(currentPeopleJuice, aCard.cost)) return confirm;
			confirm.Cancel();
			return null;
		}

		/// <summary>
		///     Creates a bin confirmation
		/// </summary>
		/// <param name="card">The card that may be binned</param>
		/// <param name="bin">The graveyard to send the binned card to</param>
		/// <returns>Reference to the created confirmation</returns>
		public Confirmation CreateBinConfirmation(CardBase card, Graveyard bin) {
			var confirm = Instantiate(confirmationPrefab.gameObject, canvas.transform).GetComponent<Confirmation>();
			confirm.card = card;
			confirm.bin = bin;
			return confirm;
		}
	}
}