using Card;

public class DuplicatingDeck : Deck {
	// Card to fill the deck with
	public CardBase card;

	// How many times to replicate the card
	public int replicationTimes;

	public new void Awake() {
		base.Awake();

		for (var i = 0; i < replicationTimes; i++) {
			var instance = Instantiate(card);
			AddCard(instance);
		}
		// cards = cards.Append(cards);
	}
}