// ReSharper disable Unity.NoNullPropagation
using UnityEngine;

public class Draggable : DraggableBase {
	private Quaternion initialRotation;
	private Vector3 resetPosition;

	public override void OnDragBegin() {
		resetPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Returns true if we are snapping, false otherwise
	public override bool OnDrag(RaycastHit? hit_) {
		if(hit_ is not null) {
			var hit = hit_.Value;
			
			if (hit.collider.CompareTag("Snappable")) {
				transform.position = hit.collider.transform.position;
				transform.rotation = hit.collider.transform.rotation;
				return true; // We are snapping
			}
		} 
		
		transform.position = GetPointerAsWorldPoint();
		transform.rotation = initialRotation;
		return false; // We are not snapping
	} 

	public override void OnDragEnd(bool shouldSnap) {
		if (shouldSnap) return;
		transform.position = resetPosition;
		transform.rotation = initialRotation;
	}
	
	
}