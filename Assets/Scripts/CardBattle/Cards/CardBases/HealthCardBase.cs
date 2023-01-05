using System.Linq;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	/// Extension to card which adds health
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class HealthCardBase : CardBase {
		/// <summary>
		/// Extension to modification which adds health
		/// </summary>
		public new class Modification : CardBase.Modification {
			public virtual HealthState GetHealth(HealthState health) => health; 	
		}
		
		/// <summary>
		/// Backing memory for health
		/// </summary>
		[SerializeField] private HealthState _health;
		/// <summary>
		/// Cache for modified health
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private HealthState? healthCache = null;
		/// <summary>
		/// The (modified) health of the card, calls <see cref="OnHealthStateChanged"/> whenever the health changes
		/// </summary>
		public HealthState healthState {
			set {
				OnHealthStateChanged(_health, value);
				_health = value;
				healthCache = null;
			} 
			get {
				healthCache ??= modifications.Aggregate(_health, (current, _mod) => {
					if(_mod is Modification mod) return mod.GetHealth(current); // If the modification touches health then run that process!
					return current;
				});
				return healthCache.Value;
			}
		}
		/// <summary>
		/// Reference to the monster's health
		/// </summary>
		public int Health => healthState.health;
		/// <summary>
		/// Reference to the current damage reduction (both permanent and temporary)
		/// </summary>
		public int DamageReduction => healthState.TotalDamageReduction;


		/// <summary>
		/// Override which also invalidates the health cache
		/// </summary>
		public override void InvalidateCaches() {
			base.InvalidateCaches();
			healthCache = null;
		}

		/// <summary>
		/// Callback called before the health of the card changes
		/// </summary>
		/// <param name="oldHealth">The old health of the card</param>
		/// <param name="newHealth">The new health of the card</param>
		public virtual void OnHealthStateChanged(HealthState oldHealth, HealthState newHealth) { }
	}
}