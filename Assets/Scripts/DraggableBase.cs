// ReSharper disable Unity.NoNullPropagation
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Base class providing abstract dragging behavior
public abstract class DraggableBase : MonoBehaviour, IPointerDownHandler {
	// Reference to the point (mouse position) action
	public InputActionReference pointerAction;
	// Reference to the click (left mouse button) action
	public InputActionReference clickAction;

	// The initial Z position (in screen space) of the card when we start dragging it.
	private float initialZ;
	// Bool if the mouse is down and selecting us
	protected bool pointerDown;
	// Bool indicating if we are currently snapping
	protected bool isSnapping;
	// The layer the object was assigned to before being dragged
	protected int initalLayer;


	// Callback functions
	// Called when we start dragging
	public virtual void OnDragBegin() { }
	// Called when we stop dragging, is passed weather or not we are currently sanpping
	public virtual void OnDragEnd(bool shouldSnap) { }
	// Called every frame while we are dragging
	public virtual bool OnDrag(RaycastHit? hit) { return false; } // Returns true if we are snapping, false otherwise


	
	// Helper functions
	// Gets the mouse position in screen space (with Z set)
	protected Vector3 GetPointer() {
		// Pixel coordinates of mouse (x,y)...
		var mousePoint2D = pointerAction.action.ReadValue<Vector2>();
		// Converted to 3D vector
		return new Vector3(mousePoint2D.x, mousePoint2D.y, initialZ);
	}
	// Gets the mouse position in world space
	protected Vector3 GetPointerAsWorldPoint() => Camera.main?.ScreenToWorldPoint(GetPointer()) ?? Vector3.zero;




	// Behavior
	// On dis/enable un/register listeners for the click and pointer actions
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
	
	// Called whenever the mouse pointer moves
	private void OnPointerMove(InputAction.CallbackContext ctx) {
		if (!pointerDown) return;
		OnPointerDrag();
	}

	// Called whenever the mouse pointer moves and we being dragged, preforms a raycast that ignores the dragged card
	//  and then calls OnDrag with the result of the raycast
	private void OnPointerDrag() {
		isSnapping = Physics.Raycast(Camera.main?.ScreenPointToRay(GetPointer()) ?? new Ray(), out var hit,
			Mathf.Infinity, ~LayerMask.GetMask("CurrentCard"))
			? OnDrag(hit)
			: OnDrag(null);
	}

	// Called when ever the click action occurs
	// Ignores all cases except releases, and cleans up after the drag (calling the OnDragEnd function)(
	private void OnClickUp(InputAction.CallbackContext ctx) {
		if (!pointerDown) return; // Don't do any processing if we aren't selected
		if (ctx.ReadValueAsButton()) return; // Don't do any processing if the mouse button is pressed at all

		pointerDown = false;
		gameObject.layer = initalLayer;

		// Invoke the callback
		OnDragEnd(isSnapping);
	}

	// Called when this object is initially clicked on, sets up the drag
	public void OnPointerDown(PointerEventData eventData) {
		// Record all of the variables we need for dragging
		pointerDown = true;
		initialZ = Camera.main?.WorldToScreenPoint(transform.position).z ?? 0;
		initalLayer = gameObject.layer;
		gameObject.layer = LayerMask.NameToLayer("CurrentCard");
		
		// Invoke the callback
		OnDragBegin();
	}
}