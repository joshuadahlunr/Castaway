using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;
using Extensions;
using Shapes;
using UnityEngine;

namespace CardBattle.Containers {
	/// <summary>
	///     Extension to deck which provide a tiny bit of AI needed for monsters to seam lively
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class MonsterDeck : Deck {
		/// <summary>
		///     Reference to the card which the monster reveals to the player on each of its turns
		/// </summary>
		public readonly List<(CardBase, CardBase)> revealedCards = new();

		/// <summary>
		///     Reference to the prefab to spawn to create a targeting arrow
		/// </summary>
		public Arrow arrowPrefab;

		/// <summary>
		///     Function which makes sure that all of the cards within this deck are flagged as belonging to the monster
		/// </summary>
		/// <remarks>This is called by the <see cref="CardGameManager" /> during initialization</remarks>
		/// <param name="ownerIndex">The index of the monster within the monster's array</param>
		public void AssignOwnerToCards(int ownerIndex) {
			foreach (var card in cards) {
				card.cardOwner = ownerIndex + 1; // Assign the card to the appropriate monster (index 0 is the player)
				card.draggable.enabled = false; // Make sure monster cards can't be interacted with by the player!
			}
		}

		/// <summary>
		///     Function which reveals a card to the player
		/// </summary>
		/// <remarks>Called by the <see cref="CardGameManager" /> when the monster's turn starts</remarks>
		public void RevealCard() {
			// Pick a random card to target
			var targetableCards = CardFilterer.FilterCards(cards[0].MonsterTargetingFilters).ToList();
			if (cards[0].CanTargetPlayer)
				targetableCards.Add(null); // If the player is targetable... null is a valid target!
			var targets = targetableCards.Distinct().ToArray();
			var target = targets.Length > 0 ? targets[Random.Range(0, targets.Length)] : null;

			// Remove the revealed card from the deck
			revealedCards.Add((cards[0], target));
			RemoveCard(0);

			// Temporarily remove the cost of the card!
			revealedCards[^1].Item1.AddModification(new RemoveCostModification {
				turnsRemaining = 1
			});

			// Create a new GameObject and assign it to the variable revealHolderA
			var revealHolderOuter = new GameObject {
				name = "RevealHolderOuter", // Set the name of the GameObject
				transform = { parent = transform } // Set the parent of the GameObject to the parent of the current object
			};

			// Set the global scale of the revealHolderA GameObject
			revealHolderOuter.transform.SetGlobalScale(Vector3.one);

			// Create a new GameObject and assign it to the variable revealHolder
			var revealHolder = new GameObject {
				name = "RevealHolder",
				transform = { parent = revealHolderOuter.transform } // Set the parent of the GameObject to the revealHolderA GameObject
			};

			// Rotate the revealHolder GameObject to face the main camera
			if (Camera.main != null) revealHolder.transform.LookAt(Camera.main.transform.position);

			// Rotate the revealHolder GameObject an additional 90 degrees around the x-axis
			revealHolder.transform.rotation *= Quaternion.Euler(90, 0, 0);

			// Set the position of the revealHolder GameObject
			revealHolder.transform.position = transform.position + Vector3.up * .25f + Vector3.right * (revealedCards.Count - 1);

			// If target is not null, create an arrow GameObject and position it between the revealHolder and the last revealed card
			if (target is not null) {
				var arrow = Instantiate(arrowPrefab.gameObject, revealHolderOuter.transform).GetComponent<Arrow>();
				arrow.transform.localScale = new Vector3(.05f, .05f, .05f);
				arrow.start.transform.position = revealHolder.transform.position;
				arrow.end.transform.position = revealedCards[^1].Item2.transform.position;
			}

			// Reveal the last card in the revealedCards list
			revealedCards[^1].Item1.gameObject.SetActive(true); // Set the GameObject of the card to active
			revealedCards[^1].Item1.transform.parent = revealHolder.transform; // Set the parent of the card to the revealHolder GameObject
			revealedCards[^1].Item1.transform.localPosition = Vector3.zero; // Set the local position of the card to zero
			revealedCards[^1].Item1.transform.localRotation = Quaternion.Euler(0, 0, 0); // Set the local rotation of the card to zero
			revealedCards[^1].Item1.transform.localScale = new Vector3(.0312499f, 2, .05f);
			revealedCards[^1].Item1.collider.enabled = false; // Disable the collider of the card

			// Call the OnMonsterReveal method on the last card in the revealedCards list
			revealedCards[^1].Item1.OnMonsterReveal();
		}


		/// <summary>
		///     Function which "plays" the currently revealed card
		/// </summary>
		/// <remarks>Called by the <see cref="CardGameManager" /> at the end of the monster's turn</remarks>
		public void PlayRevealedCard() {
			// Make sure the revealed card can be interacted with again!
			foreach (var (revealedCard, target) in revealedCards) {
				revealedCard.collider.enabled = true;
				var p = revealedCard.transform.parent.parent;

				// NOTE: On target should send the card back to the graveyard (aka bottom of deck)
				revealedCard?.OnTarget(target);
				
				// Get rid of the temporary object used to reveal the card
				Destroy(p.gameObject);
			}

			revealedCards.Clear();
		}
	}
}