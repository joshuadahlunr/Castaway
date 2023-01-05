using System.Linq;
using Cost = CardBattle.PeopleJuice.Cost;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	/// Base class for cards representing an action which a crewmate or monster can perform
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class ActionCardBase : CardBase {
		/// <summary>
		/// Extension to modification which adds health
		/// </summary>
		public new class Modification : CardBase.Modification {
			public virtual Cost GetCost(Cost cost) => cost; 	
		}
		
		/// <summary>
		/// Backing memory for cost
		/// </summary>
		[SerializeField] private Cost _cost = new() { CardBattle.PeopleJuice.Types.Generic };
		/// <summary>
		/// Cache for modified cost
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private Cost costCache = null;
		/// <summary>
		/// The (modified) cost of the card
		/// </summary>
		public Cost cost {
			set {
				_cost = value;
				costCache = null;
			} 
			get {
				costCache ??= modifications.Aggregate(_cost, (current, _mod) => {
					if(_mod is Modification mod) return mod.GetCost(current); // If the modification touches cost then run that process!
					return current;
				});
				return costCache;
			}
		}


		/// <summary>
		/// Error whenever an action card is played instead of targeted (should never happen)
		/// </summary>
		public sealed override void OnPlayed() {
			Debug.LogError("Action cards can't be played, they must target!");
			var draggable = GetComponent<DraggableBase>();
			draggable.Reset();
		}
		
		
		/// <summary>
		/// Override which also invalidates the cost cache
		/// </summary>
		public override void InvalidateCaches() {
			base.InvalidateCaches();
			costCache = null;
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
				CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyDamage(damage);
				return;
			}
			
			// Reduce the damage from the damage negation and reduce the damage negation from the damage!
			target.healthState = target.healthState.ApplyDamage(damage);
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