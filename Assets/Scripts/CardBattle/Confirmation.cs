using System;
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle {
	/// <summary>
	/// Component which holds an action (card play or target) in a pending state until the player confirms the action
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Confirmation : MonoBehaviour {
		// Relevant references 
		public Card.CardBase card, target;
		public CardContainerBase snapTarget;

		/// <summary>
		/// Bool indicating if we are targeting
		/// </summary>
		public bool TargetingCard => target is not null && snapTarget is null;

		/// <summary>
		/// Bool indicating if we are snapping
		/// </summary>
		public bool TargetingZone => snapTarget is not null && target is null;


		/// <summary>
		/// Callback called when the player confirms their choice
		/// </summary>
		public void Confirm() {
			if (TargetingZone) {
				var draggable = card.GetComponent<Draggable>();
				draggable.targetPosition = snapTarget.transform.position;
				draggable.targetRotation = snapTarget.transform.rotation;

				card.container.SendToContainer(card, snapTarget);
				card.OnPlayed();
			}
			else {
				if (card is Card.ActionCardBase aCard)
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
		/// Callback called when the player cancels their choice
		/// </summary>
		public void Cancel() {
			if (TargetingZone) {
				var draggable = card.GetComponent<Draggable>();
				draggable.Reset();
			}
			else {
				var targeting = card.GetComponent<Targeting>();
				targeting.Reset();
			}

			// Get rid of ourselves once an action has been chosen!
			CardGameManager.instance.activeConfirmationExists = false;
			Destroy(gameObject);
		}
	}
}