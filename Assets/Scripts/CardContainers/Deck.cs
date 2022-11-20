using System;
using Random = UnityEngine.Random;

public class Deck : CardContainerBase {
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

	public override void AddCard(Card.CardBase card, int index = -1) {
		base.AddCard(card, index);
		
		// TODO: Represent that deck has increased in size
	}

	public override void RemoveCard(int index) {
		base.RemoveCard(index);
		
		// TODO: Represent that deck has decreased in size
	}
	
}