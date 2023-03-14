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
        ///     The representative card prototype if this card is a selkie.
        /// </summary>
        /// <value>
        ///     A value of null indicates that this card is not a selkie.
        /// </value>
        /// <remarks>
        ///     A selkie is a type of monster in the game.
        /// </remarks>
        public CardBase selkieCard;

        /// <summary>
        ///     Gets a value indicating whether this monster is a selkie.
        /// </summary>
        /// <value><c>true</c> if this instance is a selkie; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///     A selkie is a type of monster in the game. If this property returns <c>true</c>, it means that this monster is a selkie and the <see cref="selkieCard" /> property contains a reference to the representative card prototype for the selkie.
        /// </remarks>
        public bool IsSelkie => selkieCard != null;


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