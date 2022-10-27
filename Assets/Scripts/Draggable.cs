// ReSharper disable Unity.NoNullPropagation
using UnityEngine;

public class Draggable : DraggableBase {
	public float moveSpeed = .1f;
	public float rotationSpeed = 20;
	private Quaternion initialRotation;
	private Vector3 resetPosition;

	public void Start() {
		targetPosition = transform.position;
		targetRotation = transform.rotation;
	}
	
	public override void OnDragBegin() {
		resetPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	// Returns true if we are snapping, false otherwise
	public override bool OnDrag(RaycastHit? hit_) {
		if(hit_ is not null) {
			var hit = hit_.Value;
			
			if (hit.collider.CompareTag("Snappable")) {
				targetPosition = hit.collider.transform.position;
				targetRotation = hit.collider.transform.rotation;
				return true; // We are snapping
			}
		} 
		
		targetPosition = GetPointerAsWorldPoint();
		targetRotation = initialRotation;
		return false; // We are not snapping
	} 

	public override void OnDragEnd(bool shouldSnap) {
		if (shouldSnap) return;
		targetPosition = resetPosition;
		targetRotation = initialRotation;
	}

	public void Update() {
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
	}
	
	
}