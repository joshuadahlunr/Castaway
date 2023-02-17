﻿using CardBattle.Containers;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	/// Extension to health card that represents one of the monsters on the field
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class MonsterCardBase : HealthCardBase {
		/// <summary>
		/// Reference to the deck associated with this monster
		/// </summary>
		public MonsterDeck deck;

		/// <summary>
		/// Bool indicating if the card is a swarm or not
		/// </summary>
		public virtual bool IsSwarm => false;
		/// <summary>
		/// The representative card prototype if this card is a selkie
		/// </summary>
		/// <remarks>A value of null indicates that this card is not a selkie!</remarks>
		public CardBase selkieCard = null;

		/// <summary>
		/// When the monster's health falls below 0 it has been defeated and is disabled (and we trigger a win/lose check in the card game manager)
		/// </summary>
		/// <param name="oldHealth"></param>
		/// <param name="newHealth"></param>
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