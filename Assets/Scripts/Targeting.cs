// ReSharper disable Unity.NoNullPropagation
using UnityEngine;

// Class that provides the ability to target "targetable" card with a selection arrow
public class Targeting : DraggableBase {
	// Arrow prefab
	public GameObject arrowPrefab;
	// Instance of the arrow prefab in this scene
	private static Shapes.Arrow arrowPrefabInstance;
	
	// The position initial position when we started dragging
	private Vector3 initPosition;

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
		var offset = (GetPointerAsWorldPoint() - initPosition) / 30;
		transform.position = initPosition + offset;

		// If we hit something...
		if(hit_ is not null) {
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
		
		transform.position = initPosition;
	}



	// Finds or instantiates the arrow object
	private Shapes.Arrow GetArrow() {
		arrowPrefabInstance ??= Instantiate(arrowPrefab).GetComponent<Shapes.Arrow>();
		arrowPrefabInstance.transform.localScale = new Vector3(.05f, .05f, .05f);
		return arrowPrefabInstance;
	}
}