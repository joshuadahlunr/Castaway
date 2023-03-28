using CardBattle.Containers;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	///     Represents a monster card on the field.
	/// </summary>
	/// <remarks>
	///     This class extends the HealthCardBase class and adds functionality specific to monsters.
	/// </remarks>
	/// <author>Joshua Dahl</author>
	public class MonsterCardBase : HealthCardBase {
		/// <summary>
		///     The deck associated with this monster.
		/// </summary>
		public MonsterDeck deck;

		/// <summary>
		///     Gets a value indicating whether this card represents a swarm.
		/// </summary>
		/// <value><c>true</c> if this instance is a swarm; otherwise, <c>false</c>.</value>
		/// <remarks>
		///     This property can be overridden in derived classes to indicate whether the monster is a swarm or not.
		/// </remarks>
		public virtual bool IsSwarm => false;
		

		/// <summary>
		///     Called when the monster's health state changes.
		/// </summary>
		/// <param name="oldHealth">The old health state.</param>
		/// <param name="newHealth">The new health state.</param>
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