using UnityEngine;

namespace Card {
	public class ActionCardBase : CardBase {
		// Error whenever an action card is played instead of targeted (should never happen)
		public sealed override void OnPlayed() {
			Debug.LogError("Action cards can't be played, they must target!");
			var draggable = GetComponent<DraggableBase>();
			draggable.Reset();
		}
		
		
		
		// ---- Helper Utilities ----
		
		// Utility function which handles dealing damage to a health card base or to the player if no health card base can be found
		public void DamageTargetOrPlayer(int damage, HealthCardBase target = null) {
			if (target is null) {
				CardGameManager.instance.playerHealth -= damage;
				return;
			}

			target.health -= damage;
		}
	}
}