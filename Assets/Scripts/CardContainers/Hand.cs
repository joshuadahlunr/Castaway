using Card;

public class Hand : CardContainerBase {
	public override Facing facing => Facing.FaceUp;

	public override void AddCard(Card.CardBase card, int index = -1) {
		base.AddCard(card, index);

		// Mark the card as now being in hand
		card.state |= CardBase.State.InHand;

		// TODO: Attach script that properly lays out the cards
	}

	public override void RemoveCard(int index) {
		// Mark the card as no longer being in hand
		cards[index].state &= ~CardBase.State.InHand;
		
		base.RemoveCard(index);

		// TODO: When a card is removed, attach script to properly
	}
}