using System.Linq;
using UnityEngine;

namespace Card {
	public class HealthCardBase : CardBase {
		public new class Modification : CardBase.Modification {
			public virtual int GetHealth(int health) => health; 	
		}
		
		[SerializeField] private string _health;
		private string healthCache = null;
		public string health {
			set {
				_health = value;
				healthCache = null;
			} 
			get {
				healthCache ??= modifications.Aggregate(_health, (current, mod) => mod.GetName(current));
				return healthCache;
			}
		}
		
		public override void InvalidateCaches() {
			base.InvalidateCaches();
			healthCache = null;
		}
	}
}