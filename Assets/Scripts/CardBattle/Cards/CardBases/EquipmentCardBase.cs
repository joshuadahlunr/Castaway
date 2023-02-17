using System;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	/// Extension to health card that represents an equipment card that can be placed on the field
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class EquipmentCardBase : HealthCardBase {
		public ActionCardBase associatedCardPrefab;
		
		public float arc;

		public override void OnPlayed() {
			// When played create the associated
			if (associatedCardPrefab is null) return;
			var spawned = Instantiate(associatedCardPrefab);
			CardGameManager.instance.playerHand.AddCard(spawned);
		}

		public override void OnTarget(Card.CardBase target) => throw new NotSupportedException("Equipment cards can't target!");

		public override void OnHealthStateChanged(HealthState oldHealth, HealthState newHealth) {
			if (oldHealth == newHealth) return; // Ignore this function unless the health changed
			base.OnHealthStateChanged(oldHealth, newHealth);
			
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