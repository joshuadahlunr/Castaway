// ReSharper disable Unity.NoNullPropagation
using UnityEngine;

public class Targeting : DraggableBase {
	public GameObject arrowPrefab;
	private Shapes.Arrow arrowPrefabInstance;
	
	private Vector3 resetPosition;

	public override void OnDragBegin() {
		resetPosition = transform.position;
	}

	// Returns true if we are snapping, false otherwise
	public override bool OnDrag(RaycastHit? hit_) {
		GetArrow().gameObject.SetActive(false);

		var offset = (GetPointerAsWorldPoint() - resetPosition) / 30;
		transform.position = resetPosition + offset;

		if(hit_ is not null) {
			var hit = hit_.Value;
			
			if (hit.collider.CompareTag("Targetable")) {
				GetArrow().gameObject.SetActive(true);
				GetArrow().start.transform.position = transform.position;
				GetArrow().end.transform.position = hit.collider.transform.position;
				GetArrow().Regenerate();
				return true; // We are snapping
			}
		} 

		return false; // We are not snapping
	} 

	public override void OnDragEnd(bool shouldSnap) {
		GetArrow().gameObject.SetActive(false);
		
		transform.position = resetPosition;
	}



	// Finds or instantiates the arrow object
	private Shapes.Arrow GetArrow() {
		arrowPrefabInstance ??= Instantiate(arrowPrefab).GetComponent<Shapes.Arrow>();
		arrowPrefabInstance.transform.localScale = new Vector3(.05f, .05f, .05f);
		return arrowPrefabInstance;
	} 
	
	
}