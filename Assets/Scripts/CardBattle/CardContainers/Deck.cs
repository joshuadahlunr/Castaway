using System;
using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;
using Extensions;
using SQLite;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardBattle.Containers {
	/// <summary>
	///     Deck of cards which can be visualized in the world
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Deck : CardContainerBase {
		/// <summary>
		///     Decks are face down containers
		/// </summary>
		public override Facing facing => Facing.FaceDown;

		/// <summary>
		///     Type used to store a decklist in the SQL database
		/// </summary>
		public class DeckList {
			/// <summary>
			///     The id of list, acts as an index into the DeckListCards table
			/// </summary>
			[PrimaryKey, Unique, AutoIncrement]
			public int id { set; get; }

			/// <summary>
			///     Name of the decklist in the database
			/// </summary>
			public string name { set; get; }
		}

		/// <summary>
		///     Represents a card in a decklist stored in a SQL database.
		/// </summary>
		public class DeckListCard {
			/// <summary>
			///     Gets or sets the unique identifier of the card.
			/// </summary>
			[PrimaryKey, Unique, AutoIncrement]
			public int id { set; get; }

			/// <summary>
			///     Gets or sets the identifier of the decklist to which this card belongs.
			/// </summary>
			[NotNull]
			public int listID { set; get; }

			/// <summary>
			///     Gets or sets the name of the card.
			/// </summary>
			[NotNull]
			public string name { set; get; }

			/// <summary>
			///     Gets or sets the name of the character associated with this card.
			/// </summary>
			public int? associatedCrewmateID { set; get; } // TODO: Replace with character id?

			/// <summary>
			///     Gets or sets the level of the card.
			/// </summary>
			[NotNull]
			public int level { set; get; } // TODO: Can we look this up from the character ID?
		}

		/// <summary>
		///     Card database of Unity objects that loads are drawn from, allows mapping from the SQL names to Unity objects
		/// </summary>
		public CardDatabase cardDB;

		/// <summary>
		///     List of card prototypes to inject into the deck whenever it is loaded
		/// </summary>
		public CardBase[] injectCardPrototypes;

		/// <summary>
		///     Variable the represents the initial scale of the object, as cards are added and removed from the deck this value is changed
		/// </summary>
		private float initalYScale;

		/// <summary>
		///     Reference to the attached mesh renderer
		/// </summary>
		private MeshRenderer r;

		/// <summary>
		///     When the game starts save the initial scale and then scale the deck to match the number of cards currently in it
		/// </summary>
		public void Awake() {
			initalYScale = transform.localScale.y;
			r = GetComponent<MeshRenderer>();
			UpdateDeckHeight();
		}

#if UNITY_EDITOR
		public void Update() {
			if (injectCardPrototypes == null) return;

			// If there are any cards in the inject list, add them to the front of the deck
			foreach (var cardPrototype in injectCardPrototypes)
				AddCard(Instantiate(cardPrototype), 0);

			injectCardPrototypes = null; // Empty the list of cards to inject
		}
#endif


		/// <summary>
		///     Function which can be called to load a decklist from the SQL database
		/// </summary>
		/// <param name="name">The name of the decklist to load (defaults to "Player Deck")</param>
		/// <param name="clear">If this parameter is true we will remove all cards currently in this deck before loading, if this is false we can additively load cards to the deck</param>
		/// <exception cref="ArgumentException"></exception>
		public void DatabaseLoad(string name = "Player Deck", bool clear = true) {
			// Get the table out of the database and find the decklist with the given name!
			var deckList = DatabaseManager.GetOrCreateTable<DeckList>().FirstOrDefault(l => l.name == name);
			if (deckList is null)
				throw new ArgumentException($"The decklist {name} could not be found in the database!");

			DatabaseLoad(deckList.id, clear);
		}

		/// <summary>
		///     Loads cards from the database with the given decklist ID and adds them to the deck.
		/// </summary>
		/// <param name="decklistID">The ID of the decklist to load cards from.</param>
		/// <param name="clear">Whether to remove all existing cards in the deck before loading the new cards.</param>
		public virtual void DatabaseLoad(int decklistID, bool clear = true) {
			// Get all cards in the decklist with the given ID from the database
			var cards = DatabaseManager.GetOrCreateTable<DeckListCard>().Where(card => card.listID == decklistID);

			// If we are clearing, remove all of the cards currently in the deck
			if (clear) RemoveAllCards();

			// If there are any cards in the inject list, add them to the front of the deck
			foreach (var cardPrototype in injectCardPrototypes)
				AddCard(Instantiate(cardPrototype), 0);
			injectCardPrototypes = null;

			// Use the associated cardDB to load the cards from the database
			foreach (var card in cards) {
				Debug.Log(card.name);
				// Instantiate the card from the associated cardDB
				var instantiated = cardDB.Instantiate(card.name);

				// Associate the card with a crewmate (if the database says it should be!)
				instantiated.associatedCrewmate = card.associatedCrewmateID;

				// Add a level modification to the card
				instantiated.AddModification(new LevelModification(card.level));

				// Add the instantiated card to the deck
				AddCard(instantiated);
			}
		}


		/// <summary>
		///     Algorithm which shuffles the deck
		/// </summary>
		/// <remarks>From: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle//The_modern_algorithm</remarks>
		private void YatesShuffle() {
			var n = cards.Count - 2;
			for (var i = 0; i < n; i++) {
				var j = Random.Range(i, cards.Count);
				Swap(i, j);
			}
		}

		/// <summary>
		///     Algorithm which shuffles the deck and then tries to ensure that we don't have pockets of the same card.
		/// </summary>
		public void Shuffle() {
			// TODO: This code works in python but needs to be tested more in C#

			// Actually shuffle the deck
			YatesShuffle();

			// This loop finds elements next to each-other and tries to rerandomize them
			var n = cards.Count;
			// Variables which track the last index and how many times we have swapped that index
			int iLast = -1, iLastCount = 0;
			for (var i = 1; i < n; i++) {
				// If we have swapped the same index more than 10 times (say a deck is all the same card)
				// Give up on fixing the shuffle
				if (iLastCount > 10)
					return;

				// If there are two of the same element twice in a row
				if (cards[i - 1].name == cards[i].name) {
					// Apply a random offset to all of the swaps so they aren't exact
					var offset = Random.Range(2, n / 10);
					if (offset == i)
						offset = Math.Max(offset + 2,
							n - 1); // If the offset is i make it bigger (maxing out at the total size of the deck)
					offset = Math.Clamp(offset, 0, n - 1);

					// Swap it to the other end of the deck (if we can)
					if (n - i + offset < cards.Count)
						Swap(i - 1, n - i + offset);
					// If we can't swap it to the other end, swap it with the beginning
					else Swap(offset, i);

					// And scan the array again... tracking how many times we have swapped the same index
					if (iLastCount > 0 && iLast == i)
						iLastCount++;
					else {
						iLast = i;
						iLastCount = 1;
					}

					// Scanning again means resetting current index to 1 (0 here since the next step increments)
					i = 0;
				}
			}
		}


		/// <summary>
		///     Function which updates the deck's Y scale to match how many cards are currently within the deck
		/// </summary>
		protected void UpdateDeckHeight() {
			var scale = transform.localScale;
			scale.y = Mathf.Max(initalYScale * cards.Count, initalYScale);
			transform.ChangeParentScale(scale);
		}

		/// <summary>
		///     Override of AddCard which flags cards added to the deck as inactive and updates the height of the deck
		/// </summary>
		/// <param name="card">The card to be added</param>
		/// <param name="index">Optional index indicating where it should be inserted</param>
		public override void AddCard(CardBase card, int index = -1) {
			base.AddCard(card, index);

			card.state |= CardBase.State.Inactive;

			UpdateDeckHeight();
		}

		/// <summary>
		///     Override of AddCard which unflags cards as inactive and updates the height of the deck
		/// </summary>
		/// <param name="name">The name of the card to remove</param>
		public override void RemoveCard(int index) {
			cards[index].state &= ~CardBase.State.Inactive;
			base.RemoveCard(index);

			UpdateDeckHeight();
		}
	}
}