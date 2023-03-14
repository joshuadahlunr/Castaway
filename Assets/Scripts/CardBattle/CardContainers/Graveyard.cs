using CardBattle.Card;

namespace CardBattle.Containers {
	/// <summary>
	///     The Graveyard class represents a deck of cards that have been discarded by players.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Graveyard : Deck {
		/// <summary>
		///     Adds a card to the graveyard deck.
		/// </summary>
		/// <param name="card">The card to add to the deck.</param>
		/// <param name="index">The index at which to insert the card. Default is -1, which means add to the end of the deck.</param>
		public override void AddCard(CardBase card, int index = -1) {
			// Call the AddCard method from the base class Deck
			base.AddCard(card, index);

			// Check if the added card has an ActionDraggable component and if it's currently being dragged
			var targeting = card.GetComponent<ActionDraggable>();
			if (targeting is not null && targeting.IsDragging)
				// If so, call the OnDragEnd method of the component with the argument false
				targeting.OnDragEnd(false);

			// Check if there is an active confirmation dialog displayed
			if (!CardGameManager.instance.activeConfirmationExists) return;
			// If so, try to find a Confirmation component in the scene
			var confirm = FindObjectOfType<Confirmation>();
			// If no Confirmation component is found, return
			if (confirm is null) return;
			// If the card being added is the same as the card in the Confirmation component, cancel the confirmation
			if (confirm.card == card) confirm.Cancel();
		}
	}
}