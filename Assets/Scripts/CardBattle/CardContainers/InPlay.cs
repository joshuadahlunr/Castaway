namespace CardBattle.Containers {
	/// <summary>
	/// Deck of cards which can be visualized in the world
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class InPlay : CardContainerBase {
		/// <summary>
		/// Override of AddCard which flags cards added to the deck as inactive and updates the height of the deck
		/// </summary>
		/// <param name="card">The card to be added</param>
		/// <param name="index">Optional index indicating where it should be inserted</param>
		public override void AddCard(Card.CardBase card, int index = -1) {
			base.AddCard(card, index);

			card.state |= Card.CardBase.State.InPlay;
		}

		/// <summary>
		/// Override of AddCard which unflags cards as inactive and updates the height of the deck
		/// </summary>
		/// <param name="name">The name of the card to remove</param>
		public override void RemoveCard(int index) {
			cards[index].state &= ~Card.CardBase.State.InPlay;
			base.RemoveCard(index);
		}

	}
}