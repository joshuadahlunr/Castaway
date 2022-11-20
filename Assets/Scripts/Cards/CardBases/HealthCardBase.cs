using System.Linq;
using UnityEngine;

namespace Card {
	public class HealthCardBase : CardBase {
		public new class Modification : CardBase.Modification {
			public virtual int GetHealth(int health) => health; 	
		}
		
		[SerializeField] private int _health;
		private int? healthCache = null;
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
		
		public override void InvalidateCaches() {
			base.InvalidateCaches();
			healthCache = null;
		}

		public virtual void OnHealthChanged(int oldHealth, int newHealth) { }
	}
}