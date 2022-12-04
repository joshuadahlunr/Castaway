using System;
using Card;
using SQLite;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck : CardContainerBase {
	// Type used to store a decklist in the SQL database
	public class DeckList {
		[PrimaryKey, Unique] public string name { set; get; }
		public string cards { set; get; }

		[SQLite.Ignore] public string[] Cards {
			get => cards.Split("~");
			set => cards = string.Join("~", value);
		}
	}

	// Card database that loads are drawn from
	public CardDatabase cardDB;
	
	private float initalYScale;
	public void Awake() {
		initalYScale = transform.localScale.y;
		UpdateDeckHeight();
	}

	public void DatabaseLoad(string name = "Player Deck", bool clear = true) {
		// Get the table out of the database and find the decklist with the given name!
		var deckList = DatabaseManager.GetOrCreateTable<DeckList>().FirstOrDefault(l => l.name == name);
		if (deckList is null) throw new ArgumentException($"The decklist {name} could not be found in the database!");
		
		// If we are clearing, remove all of the cards currently in the deck
		if(clear) RemoveAllCards();
		
		// Use the associated cardDB to load the cards from the database
		foreach (var card in deckList.Cards) {
			Debug.Log(card);
			AddCard(cardDB.Instantiate(card));
		}
	}

	// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle//The_modern_algorithm
	private void YatesShuffle() {
		var n = cards.Count - 2;
		for (var i = 0; i < n; i++) {
			var j = Random.Range(i, cards.Count);
			Swap(i, j);
		}
	}

	// Shuffling algorithm which tries to ensure that we don't have pockets of the same card.
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
				if (offset == i) offset = Math.Max(offset + 2, n - 1); // If the offset is i make it bigger (maxing out at the total size of the deck)

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

	protected void UpdateDeckHeight() {
		// If there are no cards in the deck, don't render it!
		if (cards.Count == 0) {
			if(GetComponent<MeshRenderer>() is { } r0) // Null check the the mesh renderer to make sure one is present!
				r0.enabled = false;
			return;
		}
		
		if(GetComponent<MeshRenderer>() is { } r)
			r.enabled = true;
		
		var scale = transform.localScale;
		scale.y = initalYScale * cards.Count;
		transform.localScale = scale;
	}

	public override void AddCard(Card.CardBase card, int index = -1) {
		base.AddCard(card, index);

		card.gameObject.SetActive(false);
		card.state |= CardBase.State.Inactive;

		UpdateDeckHeight();

		// TODO: Represent that deck has increased in size
	}

	public override void RemoveCard(int index) {
		cards[index].state &= ~CardBase.State.Inactive;
		base.RemoveCard(index);

		UpdateDeckHeight();

		// TODO: Represent that deck has decreased in size
	}
	
}