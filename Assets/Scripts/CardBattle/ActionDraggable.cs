// ReSharper disable Unity.NoNullPropagation

using CardBattle.Card;
using CardBattle.Containers;
using Shapes;
using UnityEngine;

namespace CardBattle {
	/// <summary>
	///     Class that provides the ability to target "targetable" card with a selection arrow.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class ActionDraggable : DraggableBase {
		// Inherits from DraggableBase class.

		/// <summary>
		///     Reference to arrow prefab in Unity editor.
		/// </summary>
		public GameObject arrowPrefab;

		/// <summary>
		///     Boolean indicating whether or not a target needs to be specified for the card to be played.
		/// </summary>
		public bool targetNeeded = true;

		/// <summary>
		///     Static reference to instantiated arrow prefab in this scene.
		/// </summary>
		private Arrow arrowPrefabInstance;

		/// <summary>
		///     Initial position of the card being dragged.
		/// </summary>
		public Vector3 initPosition { get; private set; }

		/// <summary>
		///     Called when dragging begins. Saves the initial position of the card.
		/// </summary>
		public override void OnDragBegin() { initPosition = transform.position; }

		/// <summary>
		///     Called while dragging. If we hit something targetable, create an arrow pointing to it. Also move the card slightly to provide a little bit of feedback! Returns true if we are snapping, false otherwise.
		/// </summary>
		/// <param name="hit_">Information about what the raycast hit, if anything.</param>
		/// <returns>True if we are snapping, false otherwise.</returns>
		public override bool OnDrag(RaycastHit? hit_) {
			// Hide the arrow
			GetArrow().gameObject.SetActive(false);

			// Move the card 1/30th of the amount that the mouse moved
			var offset = (GetPointerAsWorldPoint() - initPosition) / (targetNeeded ? 30 : 1);
			transform.position = initPosition + offset;

			// If we hit something...
			if (hit_ is not null) {
				var hit = hit_.Value;

				// And it's targetable, show the arrow being drawn from us to it
				if (hit.collider.CompareTag("Targetable")) {
					GetArrow().gameObject.SetActive(true);
					GetArrow().start.transform.position = transform.position;
					GetArrow().end.transform.position = hit.collider.transform.position;
					return true; // We are snapping
				}
			}

			return false; // We are not snapping
		}

		/// <summary>
		///     Called when the user releases the mouse click after dragging the card.
		/// </summary>
		/// <param name="shouldSnap">Determines if the card should snap back to its initial position or snap to a new location.</param>
		public override void OnDragEnd(bool shouldSnap) {
			// Hide the arrow
			GetArrow().gameObject.SetActive(false);

			// If we are snapping...
			if (isSnapping) {
				// Check if the snap object is a graveyard
#if (!DISABLE_ATTACK_BINNING)
				var graveyard = snapObject.GetComponent<Graveyard>();
				if (graveyard is not null) {
					// If it is, create a confirmation dialog to confirm the action
					CardGameManager.instance.CreateBinConfirmation(card, graveyard);
					return;
				}
#endif

				// Check if the snap object is a card and if a target is needed
				var target = snapObject.GetComponent<CardBase>();
				if (targetNeeded && target is not null) {
					// If it is, create a confirmation dialog to confirm the action
					CardGameManager.instance.CreateTargetConfirmation(card, target);
					return;
				}
			}

			// If we're not snapping and the card has moved, create a confirmation dialog to confirm the action
			if (!targetNeeded && (card.transform.position - initPosition).magnitude > .1f)
				CardGameManager.instance
					.CreateTargetConfirmation(card, card); // Emulate not targeting by targeting itself...
			// Otherwise, reset the card to its initial position
			else Reset();
		}

		/// <summary>
		///     Snaps the card back to its initial position and hides the arrow object.
		/// </summary>
		public override void Reset() {
			// Hide the arrow
			GetArrow().gameObject.SetActive(false);
			// Snap the card back to its initial position
			transform.position = initPosition;
		}

		/// <summary>
		///     Finds or instantiates the arrow object and sets its local scale.
		/// </summary>
		/// <returns>The arrow object.</returns>
		private Arrow GetArrow() {
			// If the arrow prefab instance doesn't exist, instantiate it
			arrowPrefabInstance ??= Instantiate(arrowPrefab).GetComponent<Arrow>();
			// Set the local scale of the arrow
			arrowPrefabInstance.transform.localScale = new Vector3(.05f, .05f, .05f);
			// Return the arrow
			return arrowPrefabInstance;
		}
	}
}