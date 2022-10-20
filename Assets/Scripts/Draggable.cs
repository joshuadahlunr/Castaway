// ReSharper disable Unity.NoNullPropagation

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Draggable : DraggableBase {
	private Vector3 offset;
	private Quaternion initialRotation;
	private Vector3 resetPosition;

	public override void OnDragBegin() {
		Debug.Log("Down");
		
		resetPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Returns true if we are snapping, false otherwise
	public override bool OnDrag(RaycastHit? hit_) {
		Debug.Log("Hit");
		
		if(hit_ is not null) {
			var hit = hit_.Value;
			
			if (hit.collider.CompareTag("Snappable")) {
				transform.position = hit.collider.transform.position;
				transform.rotation = hit.collider.transform.rotation;
				return true; // We are snapping
			}
		} 
		
		transform.position = GetPointerAsWorldPoint() + offset;
		transform.rotation = initialRotation;
		return false; // We are not snapping
	} 

	public override void OnDragEnd(bool shouldSnap) {
		Debug.Log("Up");
		
		if (shouldSnap) return;
		transform.position = resetPosition;
		transform.rotation = initialRotation;
	}
	
	
}