using UnityEngine;

namespace Card {
	public class MonsterCardBase : HealthCardBase {
		public MonsterDeck deck;

		public override void OnHealthChanged(int oldHealth, int newHealth) {
			if (newHealth <= 0) {
				Debug.Log($"Monster {name} defeated!");
				
				gameObject.SetActive(false); // TODO: Implement defeat logic
			}
			
			Debug.Log($"{name} took {oldHealth - newHealth} damage");
		}
	}
}