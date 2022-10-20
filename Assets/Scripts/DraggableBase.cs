// ReSharper disable Unity.NoNullPropagation

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class DraggableBase : MonoBehaviour, IPointerDownHandler {
	public InputActionReference pointerAction;
	public InputActionReference clickAction;

	private float initialZ;
	protected bool pointerDown;
	protected bool isSnapping;


	// Callback functions
	public virtual void OnDragBegin() { }
	public virtual void OnDragEnd(bool shouldSnap) { }
	public virtual bool OnDrag(RaycastHit? hit) { return false; } // Returns true if we are snapping, false otherwise


	
	// Helper functions
	protected Vector3 GetPointer() {
		// Pixel coordinates of mouse (x,y)...
		var mousePoint2D = pointerAction.action.ReadValue<Vector2>();
		// Converted to 3D vector
		return new Vector3(mousePoint2D.x, mousePoint2D.y, initialZ);
	}
	protected Vector3 GetPointerAsWorldPoint() => Camera.main?.ScreenToWorldPoint(GetPointer()) ?? Vector3.zero;

	protected Vector3 CalculateDragOffset() => transform.position - GetPointerAsWorldPoint();


	
	
	// Behavior
	public void OnEnable() {
		pointerAction.action.Enable();
		clickAction.action.Enable();

		pointerAction.action.performed += OnPointerMove;
		clickAction.action.performed += OnClickUp;
	}

	public void OnDisable() {
		pointerAction.action.performed -= OnPointerMove;
		clickAction.action.performed -= OnClickUp;
	}
	
	private void OnPointerMove(InputAction.CallbackContext ctx) {
		if (!pointerDown) return;
		OnPointerDrag();
	}

	private void OnPointerDrag() {
		isSnapping = Physics.Raycast(Camera.main?.ScreenPointToRay(GetPointer()) ?? new Ray(), out var hit,
			Mathf.Infinity, /*LayerMask.NameToLayer("Card")*/ ~(1 << 6))
			? OnDrag(hit)
			: OnDrag(null);
	}

	private void OnClickUp(InputAction.CallbackContext ctx) {
		if (ctx.ReadValueAsButton()) return;

		pointerDown = false;

		OnDragEnd(isSnapping);
	}

	public void OnPointerDown(PointerEventData eventData) {
		pointerDown = true;
		initialZ = Camera.main?.WorldToScreenPoint(transform.position).z ?? 0;
		OnDragBegin();
	}
}