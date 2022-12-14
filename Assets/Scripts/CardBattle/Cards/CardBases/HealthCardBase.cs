using System.Linq;
using UnityEngine;

namespace Card {
	/// <summary>
	/// Extension to card which adds health
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class HealthCardBase : CardBase {
		/// <summary>
		/// Extension to modification which adds health
		/// </summary>
		public new class Modification : CardBase.Modification {
			public virtual int GetHealth(int health) => health; 	
		}
		
		/// <summary>
		/// Backing memory for health
		/// </summary>
		[SerializeField] private int _health;
		/// <summary>
		/// Cache for modified health
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private int? healthCache = null;
		/// <summary>
		/// The (modified) health of the card, calls <see cref="OnHealthChanged"/> whenever the health changes
		/// </summary>
		public int health {
			set {
				OnHealthChanged(_health, value);
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
		public virtual void OnHealthChanged(int oldHealth, int newHealth) { }
	}
}