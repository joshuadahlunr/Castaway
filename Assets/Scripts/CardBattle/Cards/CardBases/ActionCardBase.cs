using System.Linq;
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
		
		/// <summary>
		/// Provides convenient access to check if it is currently our turn or not
		/// </summary>
		public bool IsOurTurn => CardGameManager.instance.IsPlayerTurn;
		
		// Utility function which handles dealing damage to a health card base or to the player if no health card base can be found
		public void DamageTargetOrPlayer(int damage, HealthCardBase target = null) {
			if (target is null) {
				CardGameManager.instance.playerHealth -= damage;
				return;
			}

			target.health -= damage;
		}
		
		// Send the card back to the player's hand if any of the provided targets are null 
		// Returns true in this case indicating the calling function should return, return of false indicates that the calling function should not return
		public bool NullAndPlayerCheck(params CardBase[] targets){
			if (!OwnedByPlayer) return false;
			if (!targets.Any(target => target is null)) return false;
			
			GetComponent<DraggableBase>().Reset();
			return true;
		}
	}
}