using Card;
using UnityEngine;

/// <summary>
/// Component which holds an action (card play or target) in a pending state until the player confirms the action
/// </summary>
/// <author>Joshua Dahl</author>
public class Confirmation : MonoBehaviour {
	// Relevant references 
	public CardBase card, target;
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
		} else {
			card.OnTarget(target);
			target.OnTargeted(card);
		}
		
		// Get rid of ourselves once an action has been chosen!
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
		Destroy(gameObject);
	}
}