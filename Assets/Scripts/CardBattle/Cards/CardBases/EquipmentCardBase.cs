using System;
using System.Collections;
using CardBattle.Card.Renderers;
using UnityEngine;

namespace CardBattle.Card {
    /// <summary>
    ///     Extension to health card that represents an equipment card that can be placed on the field
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class EquipmentCardBase : HealthCardBase {
        /// <summary>
        ///     The associated card prefab that will be created when this equipment card is played
        /// </summary>
        public ActionCardBase associatedCardPrefab;

        /// <summary>
        ///     The arc of the equipment card that is used to determine which cards are colliding with it
        /// </summary>
        public float arc;

        /// <summary>
        /// Bool tracking if we should add the associated card, becomes false once the equipment has been played for the first time... it should only be able to add its card once per game!
        /// </summary>
        protected bool shouldAddCard = true;

        /// <summary>
        ///     Called when the equipment card is played
        /// </summary>
        public override void OnPlayed() {
			// If there is no associated card prefab, return without doing anything
			if (associatedCardPrefab is null) return;
			if (!shouldAddCard) return;

			// Create an instance of the associated card prefab and add it to the player's hand
			var spawned = Instantiate(associatedCardPrefab);
			CardGameManager.instance.playerHand.AddCard(spawned);

			// Disable the associated draggable!
			IEnumerator DisableNextFrame() {
				yield return null;
				draggable.enabled = false;
			}
			StartCoroutine(DisableNextFrame());

			shouldAddCard = false;
        }

        /// <summary>
        ///     Called when the equipment card is targeting a card (which is not allowed for equipment cards)
        /// </summary>
        /// <param name="target">The target card</param>
        public override void OnTarget(CardBase target)
			=> throw new NotSupportedException("Equipment cards can't target!");

        /// <summary>
        ///     Called when the health of the equipment card changes
        /// </summary>
        /// <param name="oldHealth">The old health value</param>
        /// <param name="newHealth">The new health value</param>
        public override void OnHealthStateChanged(HealthState oldHealth, HealthState newHealth) {
			// If the health hasn't changed, return without doing anything
			if (oldHealth == newHealth) return;

			// Call the base implementation of this method
			base.OnHealthStateChanged(oldHealth, newHealth);

			// If the new health is zero or less, mark the equipment card as defeated and check if all monsters have been defeated
			if (newHealth <= 0) {
				Debug.Log($"Equipment {name} defeated!");

				// Mark the monster as defeated (disabled in Unity) and check if all monsters have been defeated
				gameObject.SetActive(false);
				CardGameManager.instance.CheckWinLose();
			}

			// Log the amount of damage taken by the card
			Debug.Log($"{name} took {oldHealth - newHealth} damage");
		}

        /// <summary>
        ///     Returns an array of cards of type T that are colliding with this equipment card
        /// </summary>
        /// <typeparam name="T">The type of card to look for</typeparam>
        /// <returns>An array of cards that are colliding with this equipment card</returns>
        public T[] GetCollidingCards<T>() where T : CardBase => renderer is not Equipment equipment ? null
			: equipment.arcCollider.GetCollidingObjectsOfType<T>();

        /// <summary>
        ///     Returns an array of CardBase objects that are colliding with this equipment card
        /// </summary>
        /// <returns>An array of CardBase objects that are colliding with this equipment card</returns>
        public CardBase[] GetCollidingCards() => GetCollidingCards<CardBase>();
	}
}