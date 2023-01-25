using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Containers;
using RotaryHeart.Lib.SerializableDictionary;
using Unity.VisualScripting;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	/// Base class all card types inherit from!
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class CardBase : MonoBehaviour {
		
		/// <summary>
		/// The renderer associated with this card
		/// </summary>
		[SerializeField] private Card.Renderers.Base _renderer;
		public new Card.Renderers.Base renderer => _renderer; // Exposed as a property so that derived classes can cast it to different types
		
		/// <summary>
		/// The container the card is currently held within
		/// </summary>
		public CardContainerBase container;
		
		/// <summary>
		/// Card property, has a tag indicating what it is used for and a value
		/// </summary>
		[Serializable]
		public struct Property {
			/// <summary>
			/// List of valid tags for properties
			/// </summary>
			public enum Tag {
				Damage,
				Health,
				Block,
				Draw,
				Utility
			}

			/// <summary>
			/// The tag associated with this property
			/// </summary>
			public Tag tag;
			/// <summary>
			/// The numeric value associated with this property
			/// </summary>
			public int value;
			
			/// <summary>
			/// Properties can be implicitly converted to their value!
			/// </summary>
			public static implicit operator int(Property p) => p.value;
		}
		
		/// <summary>
		/// Serializable Dictionary that provides a list of properties relevant to the card's effect
		/// </summary>
		[Serializable]
		public class PropertyDictionary : SerializableDictionaryBase<string, Property> {
			public new PropertyDictionary Clone() => this.CloneViaSerialization();
		}
		
		
		/// <summary>
		/// Class representing a change that has been applied to the card
		/// </summary>
		[Serializable]
		public abstract class Modification {
			/// <summary>
			/// The number of turns before this modification is removed from the card...
			/// -1 turns remaining indicates that the modification is permanent
			/// </summary>
			public int turnsRemaining = -1;  
			
			/// <summary>
			/// Returns a (potentially) modified version of the card's name
			/// </summary>
			public virtual string GetName(string name) => name;
			/// <summary>
			/// Returns a (potentially) modified version of the card's art
			/// </summary>
			public virtual Sprite GetArt(Sprite art) => art;
			/// <summary>
			/// Returns a (potentially) modified version of the card's rules
			/// </summary>
			public virtual string GetRules(string rules) => rules;
			/// <summary>
			/// Returns a (potentially) modified version of the card's miscellaneous properties
			/// </summary>
			public virtual PropertyDictionary GetProperties(PropertyDictionary props) => props;
		}
		/// <summary>
		/// The list of modifications currently applied to this card
		/// </summary>
		protected List<Modification> modifications = new();

		
		
		/// <summary>
		/// List of valid states this card can be in
		/// </summary>
		[Flags]
		public enum State {
			None = 0,
			Inactive = 1 << 1, // A card is in this state when it can't be seen/like in the deck
			InPlay = 1 << 2, // When the card is on the field
			InHand = 1 << 3 // When the card is in hand
		}
		
		/// <summary>
		/// Backing field for the state of the card
		/// </summary>
		[SerializeField] private State _state = State.Inactive;
		/// <summary>
		/// The current state of the card (invokes the <see cref="OnStateChanged"/> callback when changed)
		/// </summary>
		/// <remarks>By default cards are inactive</remarks>
		public State state {
			get => _state;
			set {
				// When we change the state of the card... fire an event
				OnStateChanged(_state, value);
				_state = value;
			}
		}

		
		/// <summary>
		/// Property which determines if the card is owned by the player or not
		/// </summary>
		public bool OwnedByPlayer => cardOwner == 0; // TODO: If we change the definition of who the player is, make sure to update this property!

		/// <summary>
		/// Index representing which player or AI owns this card
		/// </summary>
		/// <remarks>Index 0 represents the player, while index 1+ represnt all of the monsters in the <see cref="CardGameManager"/>'s monster list</remarks>
		public int cardOwner;
		
		/// <summary>
		/// Bool indicating if the card is disabled
		/// </summary>
		public bool Disabled => !isActiveAndEnabled && !renderer.disabled.activeInHierarchy;

		/// <summary>
		/// Backing memory for name
		/// </summary>
		[SerializeField] private string _name;
		/// <summary>
		/// Cache for modified name
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private string nameCache = null;
		/// <summary>
		/// The (modified) name of the card
		/// </summary>
		public new string name {
			set {
				_name = value;
				nameCache = null;
			} 
			get {
				nameCache ??= modifications.Aggregate(_name, (current, mod) => mod.GetName(current));
				return nameCache;
			}
		}

		/// <summary>
		/// Backing memory for art
		/// </summary>
		[SerializeField] private Sprite _art;
		/// <summary>
		/// Cache for modified art
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private Sprite artCache = null;
		/// <summary>
		/// The (modified) card art
		/// </summary>
		public Sprite art {
			set {
				_art = value;
				artCache = null;
			} 
			get {
				artCache ??= modifications.Aggregate(_art, (current, mod) => mod.GetArt(current));
				return artCache;
			}
		}
		
		/// <summary>
		/// Backing memory for rules
		/// </summary>
		[SerializeField] private string _rules;
		/// <summary>
		/// Cache for modified rules
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private string rulesCache = null;
		/// <summary>
		/// The (modified) rules description for this card
		/// </summary>
		public string rules {
			set {
				_rules = value;
				rulesCache = null;
			} 
			get {
				rulesCache ??= modifications.Aggregate(_rules, (current, mod) => mod.GetRules(current));
				return rulesCache;
			}
		}

		/// <summary>
		/// Backing memory for properties
		/// </summary>
		[SerializeField] private PropertyDictionary _properties;
		/// <summary>
		/// Cache for modified properties
		/// </summary>
		/// <remarks>The modification operation is costly enough for there to be a benefit to caching the results!</remarks>
		private PropertyDictionary propertiesCache = null;
		/// <summary>
		/// Dictionary of (modified) miscellaneous properties that can be modified by the modifications system
		/// </summary>
		public PropertyDictionary properties {
			set {
				_properties = value;
				propertiesCache = null;
			} 
			get {
				propertiesCache ??= modifications.Aggregate(_properties, (current, mod) => mod.GetProperties(current));
				return propertiesCache;
			}
		}

		/// <summary>
		/// Function which adds a modification to the card
		/// </summary>
		/// <param name="mod">The modification to add</param>
		/// <param name="index">Index representing where in the modifications stack to add the card (negative numbers wrap around to the end of the list)</param>
		public void AddModification(Modification mod, int? index = null) {
			if(index is null) modifications.Add(mod);
			else modifications.Insert(index.Value >= 0 ? index.Value : index.Value + modifications.Count, mod);

			// Since a new modification has been added, invalidate the caches
			InvalidateCaches();
		}
		
		// TODO: Add a way to invalidate the cache from the Unity UI
		/// <summary>
		/// Function which goes through all of the attributes which can be modified and invalidates their caches 
		/// </summary>
		public virtual void InvalidateCaches() {
			nameCache = null;
			artCache = null;
			rulesCache = null;
			propertiesCache = null;
		}

		public virtual void MarkDisabled() {
			renderer.disabled.SetActive(true);
			
			if ((state & State.InHand) == 0) return; // Don't deal with the draggable if not in hand!
			var drag = GetComponent<DraggableBase>();
			if(drag is not null) drag.enabled = false;
		}

		public virtual void MarkEnabled() {
			renderer.disabled.SetActive(false);
			
			if ((state & State.InHand) == 0) return; // Don't deal with the draggable if not in hand!
			var drag = GetComponent<DraggableBase>();
			if(drag is not null) drag.enabled = true;
		}

		/// <summary>
		/// Helper function which sends the card to its owner's graveyard!
		/// </summary>
		/// <remarks>NOTE: For monsters, the graveyard is the deck associated with the monster!</remarks>
		/// <remarks>This function is one of the few helpers in the base class since every type of card will need to be sent to the graveyard... not just action cards!</remarks>
		public virtual void SendToGraveyard() {
			if (OwnedByPlayer) {
				container.SendToContainer(this, CardGameManager.instance.playerGraveyard);
			} else {
				var ownerDeck = CardGameManager.instance.monsters[cardOwner - 1].deck;
				if(container != null) container.SendToContainer(this, ownerDeck);
				else ownerDeck.AddCard(this);
				
				// Once the card has been sent back to its owner's deck, shuffle its owner's deck
				ownerDeck.Shuffle();
			}
		}

		/// <summary>
		/// Helper function which draws <see cref="number"/> cards
		/// </summary>
		/// <param name="number">The number of cards to draw</param>
		/// <exception cref="NotImplementedException">Throws a not implemented exception when called by a monster</exception>
		public virtual void Draw(int number = 1) {
			if (OwnedByPlayer)
				for (var i = 0; i < number; i++)
					CardGameManager.instance.DrawPlayerCard();
			else if (number == 1 && container is MonsterDeck deck) {
				IEnumerator DoMonsterDraw() {
					yield return new WaitForSeconds(1);
					deck.PlayRevealedCard();
					deck.RevealCard();
				}

				StartCoroutine(DoMonsterDraw());
			} else {
				throw new NotImplementedException();
				// TODO: Can monsters draw cards?
			}
		}

		
		/// <summary>
		/// Helper function which removes this card from the game!
		/// </summary>
		public void RemoveFromGame() {
			container.RemoveCard(this);
			Destroy(gameObject);
		}
		
		
		/// <summary>
		/// Function called when the player targets their graveyard with this card... sends this card to their graveyard and adds a binned attack
		/// </summary>
		public void BinCardForAttack() {
			var attack = CardGameManager.instance.InstantiateBinnedAttack();
			container.AddCard(attack);
			container.Swap(container.Index(this), container.Index(attack));
			SendToGraveyard();
		}

		// When a new card is enabled, add it to the list of cards... when it is disabled remove it from the list of cards 
		/// <summary>
		/// This list of cards acts as a cache so that all of the cards currently in the scene can be found quickly!
		/// </summary>
		public static readonly List<CardBase> ActiveCards = new();
		private void OnEnable() => ActiveCards.Add(this);
		private void OnDisable() => ActiveCards.Remove(this);
		

		/// <summary>
		/// When the game starts make sure our renderer has a reference to us
		/// </summary>
		public void Start() {
			if(renderer is not null) renderer.card = this;
		}

		
		/// <summary>
		/// Callback called when a turn starts
		/// </summary>
		public virtual void OnTurnStart() {}
		/// <summary>
		/// Callback called when a turn ends (processes modifications lifetimes)
		/// </summary>
		/// <remarks>NOTE: needs to be called in overriding classes if automatic modifier removal is expected to function!</remarks>
		public virtual void OnTurnEnd() {
			for (var i = 0; i < modifications.Count; i++) {
				var mod = modifications[i];
				if (mod.turnsRemaining == 0) {
					modifications.RemoveAt(i);
					i--;
				} else if (mod.turnsRemaining > 0) 
					mod.turnsRemaining--;
			}
		}
		/// <summary>
		/// Callback called before the card's state changes
		/// </summary>
		/// <param name="oldState">The previous state of the card</param>
		/// <param name="newState">The new state of the card</param>
		public virtual void OnStateChanged(State oldState, State newState) { }
		/// <summary>
		/// Callback called when a monster reveals this card // TODO: Implement
		/// </summary>
		public virtual void OnMonsterReveal() { }
		/// <summary>
		/// Callback called when the player begins dragging this card
		/// </summary>
		public virtual void OnDragged() { }
		/// <summary>
		/// Callback called when this card is played to the field
		/// </summary>
		public virtual void OnPlayed() { }
		/// <summary>
		/// Callback called when this card targets something
		/// </summary>
		/// <param name="target">The card which was targeted</param>
		public virtual void OnTarget(CardBase target) { }
		// 
		/// <summary>
		/// Callback called when this card is targeted by something
		/// </summary>
		/// <param name="targeter">The card which is currently targeting this card</param>
		public virtual void OnTargeted(CardBase targeter) {}
	}
}