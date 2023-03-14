using System;
using CardBattle.Card;
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle {
	/// <summary>
	///     Component which holds an action (card play or target) in a pending state until the player confirms the action
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Confirmation : MonoBehaviour {
		// Relevant references
		public CardBase card, target;
		public Graveyard bin;
		public CardContainerBase snapTarget;

		/// <summary>
		///     Component which holds an action (card play or target) in a pending state until the player confirms the action
		/// </summary>
		public bool TargetingCard => target is not null && snapTarget is null && bin is null;

		/// <summary>
		///     Bool indicating if we are snapping
		/// </summary>
		public bool TargetingZone => snapTarget is not null && target is null && bin is null;

		/// <summary>
		///     Bool indicating we are binning the card
		/// </summary>
		public bool TargetingGraveyard => bin is not null && snapTarget is null && target is null;

		/// <summary>
		///     Called when the object is created. Checks if a valid target has been passed and throws an exception if not
		/// </summary>
		public void Start() {
			// If nothing valid is targeted... immediately cancel and throw an exception
			if (!TargetingZone && !TargetingCard && !TargetingGraveyard) {
				Cancel();
				throw new ArgumentException("No valid target was passed to the confirmation!");
			}
		}

		/// <summary>
		///     Callback called when the player confirms their choice
		/// </summary>
		public void Confirm() {
			// Check the type of target and take the appropriate action
			if (TargetingZone) {
				// If targeting a zone, snap the card to the target and send it to the container
				var draggable = card.GetComponent<EquipmentDraggable>();
				draggable.targetPosition = snapTarget.transform.position;
				draggable.targetRotation = snapTarget.transform.rotation;
				card.container.SendToContainer(card, snapTarget);
				card.OnPlayed();
			} else if (TargetingGraveyard)
				// If targeting the graveyard, bin the card for attack
				card.BinCardForAttack();
			else {
				// If targeting a card, call the OnTarget method of the card and the OnTargeted method of the target
				if (card is ActionCardBase aCard)
					if (!PeopleJuice.DeductCost(ref CardGameManager.instance.currentPeopleJuice, aCard.cost))
						throw new Exception("Attempted to play card when cost is not in the pool!");

				card.OnTarget(target);
				target.OnTargeted(card);
			}

			// Disable all of the unaffordable cards
			CardGameManager.instance.OnlyEnableAffordableCards();

			// Get rid of ourselves once an action has been chosen!
			CardGameManager.instance.activeConfirmationExists = false;
			Destroy(gameObject);
		}

		/// <summary>
		///     Callback called when the player cancels their choice
		/// </summary>
		public void Cancel() {
			// Reset the draggable position of the card and destroy the confirmation object
			var draggable = card.GetComponent<DraggableBase>();
			draggable.Reset();

			// Get rid of ourselves once an action has been chosen!
			CardGameManager.instance.activeConfirmationExists = false;
			Destroy(gameObject);
		}
	}
}