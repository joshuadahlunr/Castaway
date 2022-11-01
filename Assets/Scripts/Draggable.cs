// ReSharper disable Unity.NoNullPropagation
using UnityEngine;

// Class that provides a smoothed card dragging and snapping to certain zones
[RequireComponent(typeof(Card.CardBase))]
public class Draggable : DraggableBase {
	// How fast (in units/second) cards should move
	public float moveSpeed = .1f;
	// How fast (in degrees/second) cards should rotate
	public float rotationSpeed = 20;
	
	// The rotation of the card when we started dragging
	private Quaternion initialRotation;
	// The position of the card when we started dragging
	private Vector3 resetPosition;

	// The target position and rotation of the card (provide smooth interpolation)
	private Vector3 targetPosition;
	private Quaternion targetRotation;

	// The card associated with this draggable...
	private Card.CardBase card;
	private void Awake() => card = GetComponent<Card.CardBase>();


	// Make sure the card's target position is wherever it was place in the editor when we start
	public void Start() {
		targetPosition = transform.position;
		targetRotation = transform.rotation;
	}

	// When we start dragging save the reset positions
	public override void OnDragBegin() {
		resetPosition = targetPosition;
		initialRotation = targetRotation;
	}
	
	// While we are dragging move the position of the card to the cursor or snap it to a snap zone
	// Returns true if we are snapping, false otherwise
	public override bool OnDrag(RaycastHit? hit_) {
		// If the raycast hit something...
		if(hit_ is not null) {
			var hit = hit_.Value;
			
			// And that thing is a snapzone, then snap the card to it
			if (hit.collider.CompareTag("Snappable")) {
				targetPosition = hit.collider.transform.position;
				targetRotation = hit.collider.transform.rotation;
				return true; // We are snapping
			}
		} 
		
		// Otherwise just move the card along with the the pointer
		targetPosition = GetPointerAsWorldPoint();
		targetRotation = initialRotation;
		return false; // We are not snapping
	} 

	// When we finish dragging, if we aren't in a snap zone, snap the card back to the hand
	public override void OnDragEnd(bool shouldSnap) {
		// If we are snapping to a card container, move the associated card to that container 
		if (shouldSnap) {
			CardContainerBase container;
			if((container = snapObject.GetComponent<CardContainerBase>()) is not null)
				card.container.SendToContainer(container, card);
			return;
		};
		
		targetPosition = resetPosition;
		targetRotation = initialRotation;
	}

	
	// Every frame move the card towards its target position!
	public void Update() {
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
	}
	
	
}