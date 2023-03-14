using CardBattle.Card;

namespace CardBattle.Containers {
	/// <summary>
	///     Represents a container for cards that are currently in play.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class InPlay : CardContainerBase {
		/// <summary>
		///     Adds a card to the container at the specified index (or at the end if no index is specified).
		///     Sets the InPlay flag of the card's state using the bitwise OR operator.
		/// </summary>
		/// <param name="card">The card to add to the container.</param>
		/// <param name="index">The index at which to add the card, or -1 to add the card at the end of the container.</param>
		public override void AddCard(CardBase card, int index = -1) {
			base.AddCard(card, index); // Call the AddCard method of the base class
			card.state |= CardBase.State.InPlay; // Set the InPlay flag of the card's state
		}

		/// <summary>
		///     Removes a card from the container at the specified index.
		///     Clears the InPlay flag of the card's state using the bitwise AND operator.
		///     Calls the RemoveCard method of the base class after the state is updated.
		/// </summary>
		/// <param name="index">The index of the card to remove from the container.</param>
		public override void RemoveCard(int index) {
			cards[index].state &= ~CardBase.State.InPlay; // Clear the InPlay flag of the card's state
			base.RemoveCard(index); // Call the RemoveCard method of the base class
		}
	}
}