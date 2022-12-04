using Card;
using Extensions;
using UnityEngine;

public class MonsterDeck : Deck {
	public CardBase revealedCard;

	public void AssignOwnerToCards(int ownerIndex) {
		foreach (var card in cards)
			card.cardOwner = ownerIndex + 1; // Assign the card to the appropriate monster (index 0 is the player)
	}

	public void RevealCard() {
		revealedCard = cards[0];
		RemoveCard(0);

		// TODO: Need to improve the appearance of revealed cards!
		var revealHolder = new GameObject(); // TODO: This is super inefficient and needs to be cached!
		revealedCard.gameObject.SetActive(true);
		revealHolder.transform.parent = transform;
		revealHolder.transform.SetGlobalScale(Vector3.one);
		revealHolder.transform.LookAt(Camera.main.transform.position);
		revealHolder.transform.rotation *= Quaternion.Euler(90, 0, 0);
		revealHolder.transform.position = transform.position + Vector3.up * .25f;

		revealedCard.transform.parent = revealHolder.transform;
		revealedCard.transform.localPosition = Vector3.zero;
		revealedCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
		revealedCard.GetComponent<Collider>().enabled = false; // Don't let the player interact with the revealed card!
	}

	public void PlayRevealedCard() {
		// Make sure the revealed card can be interacted with again!
		if (revealedCard is not null) {
			revealedCard.GetComponent<Collider>().enabled = true;
			
			var p = revealedCard.transform.parent;
            revealedCard.transform.parent = null;
            Destroy(p.gameObject);
		}

		// TODO: Pick a random card for it to target
		revealedCard?.OnTarget(null);
		revealedCard = null;
	}
	
	public void OnTurnStart() {
		if (!isActiveAndEnabled) return;
		if (CardGameManager.instance.IsPlayerTurn) return; // Don't do anything on the player's turn
		
		Shuffle();
		RevealCard();
	}
}
