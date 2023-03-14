using UnityEngine;
using UnityEngine.InputSystem;

namespace CardBattle {
    /// <summary>
    ///     Rotates a game object based on the mouse position.
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class ShipRotator : MonoBehaviour {
        /// <summary>
        ///     Public fields that reference InputActionReference objects for reading mouse position, click, and submit button
        /// </summary>
        public InputActionReference mousePosition, click, submit;

        /// <summary>
        ///     Enables the InputActions and adds event listeners for the click and submit actions.
        /// </summary>
        public void OnEnable() {
			// Enable the InputActions
			mousePosition.action.Enable();
			click.action.Enable();
			submit.action.Enable();

			// Add event listeners for the click and submit actions
			click.action.performed += Submit;
			submit.action.performed += Submit;
		}

        /// <summary>
        ///     Called every frame to update the rotation of the game object based on the mouse position.
        /// </summary>
        private void Update() {
			// Get the main camera
			var mainCamera = Camera.main;

			// Get the viewport position of the Ship and mouse cursor
			Vector2 pos = mainCamera.WorldToViewportPoint(CardGameManager.instance.ship.transform.position);
			Vector2 mouse = mainCamera.ScreenToViewportPoint(mousePosition.action.ReadValue<Vector2>());

			// Calculate the angle of rotation based on the mouse position
			var angle = Mathf.Atan2(pos.y - mouse.y, pos.x - mouse.x) * Mathf.Rad2Deg;
			angle = Mathf.Round(angle / 30) * 30;

			// Apply the rotation to the Ship
			CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
		}

        /// <summary>
        ///     Called when the click or submit actions are performed. Logs a message and destroys the ShipRotator component.
        /// </summary>
        /// <param name="ctx">The InputAction.CallbackContext object that triggered the event.</param>
        public void Submit(InputAction.CallbackContext ctx) {
			// Log a message and destroy the ShipRotator component
			Debug.Log("Confirming rotation...");
			Destroy(this);
		}
	}
}