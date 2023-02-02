// ReSharper disable Unity.NoNullPropagation

using CardBattle.Card;
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle {
	/// <summary>
	/// Class that provides the ability to target "targetable" card with a selection arrow
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class ActionDraggable : DraggableBase {
		// Arrow prefab
		public GameObject arrowPrefab;
		/// <summary>
		/// Bool indicating weather or not a target needs to be specified for the card to be played
		/// </summary>
		public bool targetNeeded = true;

		// Instance of the arrow prefab in this scene
		private static Shapes.Arrow arrowPrefabInstance;

		// The position initial position when we started dragging
		public Vector3 initPosition { get; private set; }

		// When we start dragging save the initial position
		public override void OnDragBegin() {
			initPosition = transform.position;
		}

		// While dragging if we hit something targetable, create an arrow pointing to it
		// Also move the card slightly to provide a little bit of feedback!
		// Returns true if we are snapping, false otherwise
		public override bool OnDrag(RaycastHit? hit_) {
			// Hide the arrow
			GetArrow().gameObject.SetActive(false);

			// Move the card 1/30th of the amount that the mouse moved
			var offset = (GetPointerAsWorldPoint() - initPosition) / (targetNeeded ? 30 : 1);
			transform.position = initPosition + offset;

			// If we hit something...
			if (hit_ is not null) {
				var hit = hit_.Value;

				// And its targetable, show the arrow being drawn from us to it
				if (hit.collider.CompareTag("Targetable")) {
					GetArrow().gameObject.SetActive(true);
					GetArrow().start.transform.position = transform.position;
					GetArrow().end.transform.position = hit.collider.transform.position;
					// GetArrow().Regenerate(); // Regenerate the shape of the arc
					return true; // We are snapping
				}
			}

			return false; // We are not snapping
		}

		// When we are done dragging make sure to hide the arrow, and snap the card back to where it started
		public override void OnDragEnd(bool shouldSnap) {
			GetArrow().gameObject.SetActive(false);

			if (isSnapping) {
#if (!DISABLE_ATTACK_BINNING)
				var graveyard = snapObject.GetComponent<Graveyard>();
				if (graveyard is not null) {
					CardGameManager.instance.CreateBinConfirmation(card, graveyard);
					return;
				}
#endif

				var target = snapObject.GetComponent<Card.CardBase>();
				if (targetNeeded && target is not null) {
					CardGameManager.instance.CreateTargetConfirmation(card, target);
					return;
				}
			}
			
			if(!targetNeeded && (card.transform.position - initPosition).magnitude > .1f)
				CardGameManager.instance.CreateTargetConfirmation(card, card); // Emulate not target by targeting itself...
			else Reset();
		}

		// When we reset, snap the card back to its initial position and make sure the arrow is hidden
		public override void Reset() {
			GetArrow().gameObject.SetActive(false);
			transform.position = initPosition;
		}




		// Finds or instantiates the arrow object
		private Shapes.Arrow GetArrow() {
			arrowPrefabInstance ??= Instantiate(arrowPrefab).GetComponent<Shapes.Arrow>();
			arrowPrefabInstance.transform.localScale = new Vector3(.05f, .05f, .05f);
			return arrowPrefabInstance;
		}
	}
}