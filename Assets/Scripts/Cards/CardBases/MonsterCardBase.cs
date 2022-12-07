using UnityEngine;

namespace Card {
	public class MonsterCardBase : HealthCardBase {
		public MonsterDeck deck;

		public override void OnHealthChanged(int oldHealth, int newHealth) {
			if (newHealth <= 0) {
				Debug.Log($"Monster {name} defeated!");
				
				// Mark the monster as defeated (disabled in Unity) and check if all monsters have been defeated
				gameObject.SetActive(false);
				CardGameManager.instance.CheckWinLose();
			}
			
			Debug.Log($"{name} took {oldHealth - newHealth} damage");
		}
	}
}