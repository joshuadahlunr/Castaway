// ReSharper disable Unity.NoNullPropagation

using CardBattle.Card;
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle.Passives {
	/// <summary>
	///     Class that provides a smoothed card dragging and snapping to certain zones
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class HarpoonDraggable : DraggableBase {
		/// <summary>
		///     How fast (in units/second) cards should move
		/// </summary>
		public float moveSpeed = .1f;

		/// <summary>
		///     How fast (in degrees/second) cards should rotate
		/// </summary>
		public float rotationSpeed = 20;

		/// <summary>
		///     The rotation of the card when we started dragging
		/// </summary>
		private Quaternion initialRotation;

		/// <summary>
		///     The position of the card when we started dragging
		/// </summary>
		private Vector3 resetPosition;

		// The target position and rotation of the card (used for smooth interpolation)
		public Vector3 targetPosition;
		public Quaternion targetRotation;


		/// <summary>
		///     Make sure the card's target position is wherever it was place in the editor when we start
		/// </summary>
		public void Start() => TargetCurrentTransform();

		/// <summary>
		///     When we start dragging save the reset positions
		/// </summary>
		public override void OnDragBegin() {
			AudioManager.instance?.uiSoundFXPlayer?.PlayTrackImmediate("Interact");

			resetPosition = targetPosition;
			initialRotation = targetRotation;
		}

		/// <summary>
		///     While we are dragging move the position of the card to the cursor
		/// </summary>
		/// <param name="hit_">The potential raycast hit (null if mouse not over anything relevant)</param>
		/// <returns>Returns true if we are snapping, false otherwise</returns>
		public override bool OnDrag(RaycastHit? hit_) {
			targetPosition = GetPointerAsWorldPoint();
			targetPosition.y = .5f;
			Debug.Log(targetPosition);
			targetRotation = initialRotation;
			return false; // We are not snapping
		}

		/// <summary>
		///     When we finish dragging, if we aren't in a snap zone, snap the card back to the hand
		/// </summary>
		public override void OnDragEnd(bool shouldSnap) {
		    Debug.Log("On drag end");
			Destroy(this);
		}

		/// <summary>
		///     Reset the card back to its initial position (interpolated)
		/// </summary>
		public override void Reset() {
			targetPosition = resetPosition;
			targetRotation = initialRotation;
		}

		/// <inheritdoc cref="DraggableBase"/>
		public override void TargetCurrentTransform() {
			targetPosition = transform.position;
			targetRotation = transform.rotation;
		}

		/// <summary>
		///     Every frame move the card towards its target pose!
		/// </summary>
		public void LateUpdate() {
			transform.position = targetPosition;
			transform.rotation = targetRotation;
			// transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);
			// transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
		}
	}
}