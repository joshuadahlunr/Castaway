using System;
using UnityEngine;

namespace CardBattle {
	[Serializable]
	public struct HealthState {
		public int health;
		public int maxHealth;

		public static implicit operator int(HealthState h) => h.health;

		public int permanentDamageReduction;
		public int temporaryDamageReduction;

		public int TotalDamageReduction => permanentDamageReduction + temporaryDamageReduction;

		public HealthState ApplyDamage(int damage) {
			int temp = Math.Max(damage, 0);

			temp = Math.Max(damage - temporaryDamageReduction, 0);
			temporaryDamageReduction = Math.Max(temporaryDamageReduction - damage, 0);
			damage = temp;

			temp = Math.Max(damage - permanentDamageReduction, 0);
			permanentDamageReduction = Math.Max(permanentDamageReduction - damage, 0);
			damage = temp;

			health -= damage;
			return this;
		}

		public HealthState ApplyHealing(int healing) {
            if (healing < 0)
             	return ApplyDamage(-healing);;

			health += healing;
			return this;
		}

		public HealthState SetPermanentDamageReduction(int value) {
			permanentDamageReduction = value;
			return this;
		}

		public HealthState AddPermanentDamageReduction(int value) {
			permanentDamageReduction += value;
			return this;
		}

		public HealthState SetTemporaryDamageReduction(int value) {
			temporaryDamageReduction = value;
			return this;
		}

		public HealthState AddTemporaryDamageReduction(int value) {
			temporaryDamageReduction += value;
			return this;
		}
	}
}