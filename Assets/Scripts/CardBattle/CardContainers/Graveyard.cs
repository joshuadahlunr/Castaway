namespace CardBattle.Containers {
	public class Graveyard : Deck {

		public override void AddCard(Card.CardBase card, int index = -1) {
			base.AddCard(card, index);

			var targeting = card.GetComponent<ActionDraggable>();
			if (targeting is not null && targeting.IsDragging) 
				targeting.OnDragEnd(false);

			if (!CardGameManager.instance.activeConfirmationExists) return;
			var confirm = FindObjectOfType<Confirmation>();
			if (confirm is null) return;
			
			if(confirm.card == card) confirm.Cancel();
		}
	}
}

