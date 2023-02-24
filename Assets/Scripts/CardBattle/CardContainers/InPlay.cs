namespace CardBattle.Containers {
	public class InPlay : CardContainerBase {
		public override void AddCard(Card.CardBase card, int index = -1) {
			base.AddCard(card, index);

			card.state |= Card.CardBase.State.InPlay;
		}

		public override void RemoveCard(int index) {
			cards[index].state &= ~Card.CardBase.State.InPlay;
			base.RemoveCard(index);
		}

	}
}