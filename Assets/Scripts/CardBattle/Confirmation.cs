using Card;
using UnityEngine;

public class Confirmation : MonoBehaviour {
	public CardBase card, target;
	public CardContainerBase snapTarget;

	public bool TargetingCard => target is not null && snapTarget is null;
	public bool TargetingZone => snapTarget is not null && target is null;

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