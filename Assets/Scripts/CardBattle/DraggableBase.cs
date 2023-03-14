// ReSharper disable Unity.NoNullPropagation

using CardBattle.Card;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CardBattle {
	/// <summary>
	///     Base class providing abstract dragging behavior
	/// </summary>
	/// <author>Joshua Dahl</author>
	public abstract class DraggableBase : MonoBehaviour, IPointerDownHandler {
		/// <summary>
		///     Reference to the point (mouse position) action
		/// </summary>
		public InputActionReference pointerAction;

		/// <summary>
		///     Reference to the click (left mouse button) action
		/// </summary>
		public InputActionReference clickAction;

		/// <summary>
		///     The initial Z position (in screen space) of the card when we start dragging it.
		/// </summary>
		private float initialZ;

		/// <summary>
		///     Bool if the mouse is down and selecting us
		/// </summary>
		protected bool pointerDown;

		/// <summary>
		///     Bool indicating if we are currently snapping
		/// </summary>
		protected bool isSnapping;

		/// <summary>
		///     The object we are snapping to
		/// </summary>
		protected GameObject snapObject;

		/// <summary>
		///     The layer the object was assigned to before being dragged
		/// </summary>
		protected int initalLayer;

		/// <summary>
		///     The card associated with this draggable...
		/// </summary>
		protected CardBase card;

		/// <summary>
		///     At the start of the game, find the reference to the associated card
		/// </summary>
		protected void Awake() => card = GetComponent<CardBase>();

		/// <summary>
		///     Public bool indicating if the draggable is being dragged
		/// </summary>
		public bool IsDragging => pointerDown;


		// ---- Callback functions ----


		/// <summary>
		///     Callback called when we start dragging
		/// </summary>
		public virtual void OnDragBegin() { }

		/// <summary>
		///     Callback called when we stop dragging, is passed weather or not we are currently snapping
		/// </summary>
		/// <param name="shouldSnap">Bool representing weather or not we are currently snapping</param>
		public virtual void OnDragEnd(bool shouldSnap) { }

		/// <summary>
		///     Callback called every frame while we are dragging
		/// </summary>
		/// <param name="hit">The potential raycast hit (null if mouse not over anything relevant)</param>
		/// <returns>Returns true if we are snapping, false otherwise</returns>
		public virtual bool OnDrag(RaycastHit? hit) => false;

		/// <summary>
		///     Callback called when a confirmation fails or something isn't "targeted" should reset the card to its initial state!
		/// </summary>
		public virtual void Reset() { }


		// ---- Behavior ----


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

		/// <summary>
		///     Called whenever the mouse pointer moves
		/// </summary>
		/// <param name="_">Input action context (which is ignored)</param>
		private void OnPointerMove(InputAction.CallbackContext _) {
			if (!pointerDown) return;
			OnPointerDrag();
		}

		/// <summary>
		///     Called whenever the mouse pointer moves and we being dragged, preforms a raycast that ignores the dragged card
		///     and then calls OnDrag with the result of the raycast
		/// </summary>
		private void OnPointerDrag() {
			isSnapping = false;

			if (Physics.Raycast(Camera.main?.ScreenPointToRay(GetPointer()) ?? new Ray(), out var hit, Mathf.Infinity,
				    ~LayerMask.GetMask("CurrentCard"))) {
				snapObject = hit.collider.gameObject;
				isSnapping = OnDrag(hit);
			} else OnDrag(null);
		}


		/// <summary>
		///     Called when ever the click action occurs
		///     Ignores all cases except releases, and cleans up after the drag (calling the OnDragEnd function)
		/// </summary>
		/// <param name="ctx">Input action context (used to get if the mouse button is pressed or not)</param>
		private void OnClickUp(InputAction.CallbackContext ctx) {
			if (!pointerDown) return; // Don't do any processing if we aren't selected
			if (ctx.ReadValueAsButton()) return; // Don't do any processing if the mouse button is pressed at all

			pointerDown = false;
			gameObject.layer = initalLayer;

			// Reenable all of the valid cards!
			CardGameManager.EnableCards(CardFilterer.EnumerateAllCards());

			// Invoke the callback
			OnDragEnd(isSnapping);
		}

		/// <summary>
		///     Called when this object is initially clicked on, sets up the drag
		/// </summary>
		/// <param name="_">Input action context (ignored)</param>
		public void OnPointerDown(PointerEventData _) {
			// Record all of the variables we need for dragging
			pointerDown = true;
			initialZ = Camera.main?.WorldToScreenPoint(transform.position).z ?? 0;
			initalLayer = gameObject.layer;
			gameObject.layer = LayerMask.NameToLayer("CurrentCard");

			// Disable all of the cards this card can't target...
			CardFilterer.FilterAndDisableCards(card.TargetingFilters);
			card.MarkEnabled();

			// Invoke the callback
			OnDragBegin();
			card?.OnDragged();
		}


		// ---- Helper functions ----


		/// <summary>
		///     Gets the mouse position in screen space (with Z set)
		/// </summary>
		/// <returns>The mouse position in screen space (with Z set)</returns>
		protected Vector3 GetPointer() {
			// Pixel coordinates of mouse (x,y)...
			var mousePoint2D = pointerAction.action.ReadValue<Vector2>();
			// Converted to 3D vector
			return new Vector3(mousePoint2D.x, mousePoint2D.y, initialZ);
		}

		/// <summary>
		///     Gets the mouse position in world space
		/// </summary>
		/// <returns>The mouse position in world space</returns>
		protected Vector3 GetPointerAsWorldPoint() => Camera.main?.ScreenToWorldPoint(GetPointer()) ?? Vector3.zero;
	}
}