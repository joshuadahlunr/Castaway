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
		/// Card this creatures becomes when a selkie
		/// </summary>
		public ActionCardBase selkieCard;


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

		public void Position() {
			transform.localScale *= 2;
			var pos = CardGameManager.instance.ocean.transform.position;
			var angle = Random.Range(40, 140f);
			pos.x += Mathf.Cos(angle * Mathf.Deg2Rad);
			pos.z += Mathf.Sin(angle * Mathf.Deg2Rad);
			transform.position = pos; // TODO: Adjust so that monsters won't spawn on top of each-other!
		}


		// Function which upgrades this card into its selkie form
		public SelkieMonsterCardBase PromoteToSelkie() {
			if (this is SelkieMonsterCardBase sThis) return sThis; // If we are already a selkie no need to promote!

			var selkie = gameObject.AddComponent<SelkieMonsterCardBase>();
			selkie.name = name;
			selkie.art = art;
			selkie.state = state;
			selkie.collider = collider;
			selkie.container = container;
			selkie.deck = deck;
			selkie.draggable = draggable;
			selkie.modifications = modifications;
			selkie.cardOwner = cardOwner;
			selkie.selkieCard = selkieCard;
			selkie.healthStateChanged = healthStateChanged;
			selkie.properties = properties.Clone();
			selkie._renderer = _renderer;
			selkie.renderer.card = selkie;

			if (container != null) {
				container.AddCard(selkie, container.Index(this));
				container.RemoveCard(this);
			}

			Destroy(this);
			return selkie;
		}
	}
}