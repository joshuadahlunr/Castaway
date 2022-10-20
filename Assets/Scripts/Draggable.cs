// ReSharper disable Unity.NoNullPropagation

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour, IPointerDownHandler /*IPointerUpHandler,*/ /*IPointerMoveHandler*/ {
	public InputActionReference pointerAction;
	public InputActionReference clickAction;

	private Vector3 offset;
	private float initialZ;
	private Quaternion initialRotation;
	private bool pointerDown;
	private bool isSnapping;
	private Vector3 resetPosition;

	public void OnEnable() {
		pointerAction.action.Enable();
		clickAction.action.Enable();

		pointerAction.action.performed += ctx => OnPointerMove();
		clickAction.action.performed += ctx => { if(!ctx.ReadValueAsButton()) OnClickUp(); };
	}

	public void OnPointerDrag() {
		transform.position = GetPointerAsWorldPoint() + offset;
		transform.rotation = initialRotation;

		// var ray = Camera.main?.ScreenPointToRay(GetPointer()) ?? new Ray();
		// Debug.DrawRay(ray.origin, ray.direction.normalized * 1000, Color.blue);
		
		isSnapping = false;
		if (Physics.Raycast(Camera.main?.ScreenPointToRay(GetPointer()) ?? new Ray(), out var hit, Mathf.Infinity, /*LayerMask.NameToLayer("Card")*/ ~(1 << 6))) {
			// Debug.Log(hit.collider.name);
			if (hit.collider.CompareTag("Snappable")) {
				transform.position = hit.collider.transform.position;
				transform.rotation = hit.collider.transform.rotation;
				isSnapping = true;
			}
		} 

		Debug.Log("Drag");
	}

	public void OnPointerMove( /*PointerEventData eventData*/) {
		if (!pointerDown) return;
		OnPointerDrag();
	}

	public void OnClickUp() {
		pointerDown = false;
		Debug.Log("Up");

		if (!isSnapping) {
			transform.position = resetPosition;
			transform.rotation = initialRotation;
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		pointerDown = true;
		resetPosition = transform.position;
		initialZ = Camera.main?.WorldToScreenPoint(transform.position).z ?? 0;
		offset = transform.position - GetPointerAsWorldPoint();
		initialRotation = transform.rotation;
	}

	
	
	private Vector3 GetPointer() {
		// Pixel coordinates of mouse (x,y)...
		var mousePoint2D = pointerAction.action.ReadValue<Vector2>();
		// Converted to 3D vector
		return new Vector3(mousePoint2D.x, mousePoint2D.y, initialZ);
	}

	private Vector3 GetPointerAsWorldPoint() {
		return Camera.main?.ScreenToWorldPoint(GetPointer()) ?? Vector3.zero;
	}
}