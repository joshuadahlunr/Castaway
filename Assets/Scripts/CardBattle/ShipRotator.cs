using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CardBattle {
	public class ShipRotator : MonoBehaviour {
		public InputActionReference mousePosition;
		public InputActionReference click;
		public InputActionReference submit;

		public void OnEnable() {
			mousePosition.action.Enable();
			click.action.Enable();
			submit.action.Enable();

			click.action.performed += Submit;
			submit.action.performed += Submit;
		}

		// Update is called once per frame
		private void Update() {
			var mainCamera = Camera.main;
			Vector2 pos = mainCamera.WorldToViewportPoint(CardGameManager.instance.ship.transform.position);
			Vector2 mouse = mainCamera.ScreenToViewportPoint(mousePosition.action.ReadValue<Vector2>());
			var angle = Mathf.Atan2(pos.y - mouse.y, pos.x - mouse.x) * Mathf.Rad2Deg;
			angle = Mathf.Round(angle / 30) * 30;
			

			CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
		}

		public void Submit(InputAction.CallbackContext ctx) {
			Debug.Log("Confirming rotation...");
			Destroy(this);
		}
	}
}