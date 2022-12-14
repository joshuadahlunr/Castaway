using System.Linq;
using UnityEngine;

namespace Card {
	/// <summary>
	/// Base class for cards representing an action which a crewmate or monster can perform
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class ActionCardBase : CardBase {
		/// <summary>
		/// Error whenever an action card is played instead of targeted (should never happen)
		/// </summary>
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
		
		/// <summary>
		/// Utility function which handles dealing damage to a health card base or to the player if no health card base can be found
		/// </summary>
		/// <param name="damage">Damage to deal</param>
		/// <param name="target">Target to deal damage to (or player if null)</param>
		public void DamageTargetOrPlayer(int damage, HealthCardBase target = null) {
			if (target is null) {
				CardGameManager.instance.playerHealth -= damage;
				return;
			}

			target.health -= damage;
		}
		
		/// <summary>
		/// Send the card back to the player's hand if any of the provided targets are null 
		/// </summary>
		/// <param name="targets">List of targets to check the null status of</param>
		/// <returns>Returns true if any of the targets are null, indicating the calling function should return, return of false indicates that the calling function should not return</returns>
		public bool NullAndPlayerCheck(params CardBase[] targets){
			if (!OwnedByPlayer) return false;
			if (!targets.Any(target => target is null)) return false;
			
			GetComponent<DraggableBase>().Reset();
			return true;
		}

		/// <summary>
		/// Function which draws a card for the player (shuffling the graveyard into their deck if necessary)
		/// </summary>
		/// <remarks>Just a helper to invoke the same functionality in the card game manager</remarks>
		public void DrawPlayerCard() {
			CardGameManager.instance.DrawPlayerCard();
		}
	}
}