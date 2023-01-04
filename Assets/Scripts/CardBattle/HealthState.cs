using System;

namespace CardBattle {
	public struct HealthState {
		public int health;

		public int permanentDamageReduction;
		public int temporaryDamageReduction;

		public void ApplyDamage(int damage) {
			int temp = Math.Max(damage, 0);

			temp = Math.Max(damage - temporaryDamageReduction, 0);
			temporaryDamageReduction = Math.Max(temporaryDamageReduction - damage, 0);
			damage = temp;
			
			temp = Math.Max(damage - permanentDamageReduction, 0);
			permanentDamageReduction = Math.Max(permanentDamageReduction - damage, 0);
			damage = temp;

			health -= damage;
		}

		public void ApplyHealing(int healing) {
			if (healing < 0) {
				ApplyDamage(-healing);
				return;
			}

			health += healing;
		}
	}
}