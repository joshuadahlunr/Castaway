using Card;
using UnityEngine;

/// <summary>
/// Extension to deck which provide a tiny bit of AI needed for monsters to seam lively
/// </summary>
/// <author>Joshua Dahl</author>
public class MonsterDeck : Deck {
	/// <summary>
	/// Reference to the card which the monster reveals to the player on each of its turns
	/// </summary>
	public CardBase revealedCard;

	/// <summary>
	/// Function which makes sure that all of the cards within this deck are flagged as belonging to the monster
	/// </summary>
	/// <remarks>This is called by the <see cref="CardGameManager"/> during initialization</remarks>
	/// <param name="ownerIndex">The index of the monster within the monster's array</param>
	public void AssignOwnerToCards(int ownerIndex) {
		foreach (var card in cards)
			card.cardOwner = ownerIndex + 1; // Assign the card to the appropriate monster (index 0 is the player)
	}

	/// <summary>
	/// Function which reveals a card to the player
	/// </summary>
	/// <remarks>Called by the <see cref="CardGameManager"/> when the monster's turn starts</remarks>
	public void RevealCard() {
		revealedCard = cards[0];
		RemoveCard(0);

		// TODO: Need to improve the appearance of revealed cards!
		var revealHolder = new GameObject(); // TODO: This is super inefficient and needs to be cached!
		revealedCard.gameObject.SetActive(true);
		revealHolder.transform.parent = transform;
		revealHolder.transform.LookAt(Camera.main.transform.position);
		revealHolder.transform.rotation *= Quaternion.Euler(90, 0, 0);
		revealHolder.transform.position = transform.position + Vector3.up * .25f;

		revealedCard.transform.parent = revealHolder.transform;
		revealedCard.transform.localPosition = Vector3.zero;
		revealedCard.transform.localRotation = Quaternion.Euler(0, 0, 0);
		revealedCard.GetComponent<Collider>().enabled = false; // Don't let the player interact with the revealed card!

		revealedCard.OnMonsterReveal();
	}

	/// <summary>
	/// Function which "plays" the currently revealed card
	/// </summary>
	/// <remarks>Called by the <see cref="CardGameManager"/> at the end of the monster's turn</remarks>
	public void PlayRevealedCard() {
		// Make sure the revealed card can be interacted with again!
		if (revealedCard is not null) {
			revealedCard.GetComponent<Collider>().enabled = true;
			
			// Get rid of the temporary object used to reveal the card
			var p = revealedCard.transform.parent;
            revealedCard.transform.parent = null;
            Destroy(p.gameObject);
		}

		// TODO: Pick a random card for it to target
		revealedCard?.OnTarget(null);
		revealedCard = null;
	}
}
