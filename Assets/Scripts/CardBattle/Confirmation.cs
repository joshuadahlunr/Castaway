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
		public Graveyard bin;
		public CardContainerBase snapTarget;

		/// <summary>
		/// Component which holds an action (card play or target) in a pending state until the player confirms the action
		/// </summary>
		public bool TargetingCard => target is not null && snapTarget is null && bin is null;

		/// <summary>
		/// Bool indicating if we are snapping
		/// </summary>
		public bool TargetingZone => snapTarget is not null && target is null && bin is null;

		/// <summary>
		/// Bool indicating we are binning the card
		/// </summary>
		public bool TargetingGraveyard => bin is not null && snapTarget is null && target is null;

		public void Start() {
			// if(true /* should auto confirm*/)
			// 	Confirm();
		}


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
			} else if (TargetingGraveyard) 
				card.BinCardForAttack();
			else {
				if (card is Card.ActionCardBase aCard)
					if (!PeopleJuice.DeductCost(ref CardGameManager.instance.currentPeopleJuice, aCard.cost))
						throw new Exception("Attempted to play card when cost is not in the pool!");

				card.OnTarget(target);
				target.OnTargeted(card);
			}
			
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
			} else {
				var targeting = card.GetComponent<Targeting>();
				targeting.Reset();
			}

			// Get rid of ourselves once an action has been chosen!
			CardGameManager.instance.activeConfirmationExists = false;
			Destroy(gameObject);
		}
	}
}