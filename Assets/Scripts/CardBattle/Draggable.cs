// ReSharper disable Unity.NoNullPropagation
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle {

	/// <summary>
	/// Class that provides a smoothed card dragging and snapping to certain zones
	/// </summary>
	/// <author>Joshua Dahl</author>
	[RequireComponent(typeof(Card.CardBase))]
	public class Draggable : DraggableBase {
		/// <summary>
		/// How fast (in units/second) cards should move
		/// </summary>
		public float moveSpeed = .1f;

		/// <summary>
		/// How fast (in degrees/second) cards should rotate
		/// </summary>
		public float rotationSpeed = 20;

		/// <summary>
		/// The rotation of the card when we started dragging
		/// </summary>
		private Quaternion initialRotation;

		/// <summary>
		/// The position of the card when we started dragging
		/// </summary>
		private Vector3 resetPosition;

		// The target position and rotation of the card (used for smooth interpolation)
		public Vector3 targetPosition;
		public Quaternion targetRotation;


		/// <summary>
		/// Make sure the card's target position is wherever it was place in the editor when we start
		/// </summary>
		public void Start() {
			targetPosition = transform.position;
			targetRotation = transform.rotation;
		}

		/// <summary>
		/// When we start dragging save the reset positions
		/// </summary>
		public override void OnDragBegin() {
			resetPosition = targetPosition;
			initialRotation = targetRotation;
		}

		/// <summary>
		/// While we are dragging move the position of the card to the cursor or snap it to a snap zone
		/// </summary>
		/// <param name="hit_">The potential raycast hit (null if mouse not over anything relevant)</param>
		/// <returns>Returns true if we are snapping, false otherwise</returns>
		public override bool OnDrag(RaycastHit? hit_) {
			// If the raycast hit something...
			if (hit_ is not null) {
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

		/// <summary>
		/// When we finish dragging, if we aren't in a snap zone, snap the card back to the hand
		/// </summary>
		public override void OnDragEnd(bool shouldSnap) {
			// If we are snapping to a card container, move the associated card to that container 
			if (shouldSnap) {
				CardContainerBase container;
				if ((container = snapObject.GetComponent<CardContainerBase>()) is not null) {
					CardGameManager.instance.CreateSnapConfirmation(card, container);
					return;
				}
			}

			;

			Reset();
		}

		/// <summary>
		/// Reset the card back to its initial position (interpolated)
		/// </summary>
		public override void Reset() {
			targetPosition = resetPosition;
			targetRotation = initialRotation;
		}

		/// <summary>
		/// Every frame move the card towards its target pose!
		/// </summary>
		public void Update() {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
		}
	}
}