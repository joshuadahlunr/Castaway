using System;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Unity.VisualScripting;
using UnityEngine;

namespace Card {
	// Base class all card types can inherit from!
	public class CardBase : MonoBehaviour {
		
		// The renderer associated with this card
		[SerializeField] private Card.Renderers.Base _renderer;
		public new Card.Renderers.Base renderer => _renderer; // Exposed as a property so that derived classes can cast it to different types
		// The container the card is currently held within
		public CardContainerBase container;

		// Card property, has a tag indicating what it is used for and a value
		[Serializable]
		public struct Property {
			public enum Type {
				Damage,
				Health,
				Block,
				Draw,
				Utility
			}

			public Type type;
			public int value;
			
			// Properties can be implicitly converted to their value!
			public static implicit operator int(Property p) => p.value;
		}
		// Dictionary that provides a list of properties relevant to the card's effect
		[Serializable]
		public class PropertyDictionary : SerializableDictionaryBase<string, Property> {
			public new PropertyDictionary Clone() => this.CloneViaSerialization();
		}
		
		
		// Class representing a change that has been applied to the card
		// TODO: Implement turns remaining removal
		[Serializable]
		public abstract class Modification {
			public int turnsRemaining = -1; // -1 turns remaining indicates that the modification is permanent 
			
			public virtual string GetName(string name) => name; 
			public virtual string GetCost(string cost) => cost; 
			public virtual Sprite GetArt(Sprite art) => art;
			public virtual string GetRules(string rules) => rules;
			public virtual PropertyDictionary GetProperties(PropertyDictionary props) => props;
		}
		// The list of modifications currently applied to this card
		protected List<Modification> modifications = new();

		

		// List of valid states this card can be in
		[Flags]
		public enum State {
			None = 0,
			Inactive = 1 << 1, // A card is in this state when it can't be seen/like in the deck
			InPlay = 1 << 2, // When the card is on the field
			InHand = 1 << 3 // When the card is in hand
		}
		// By default cards are inactive
		[SerializeField] private State _state = State.Inactive;
		public State state {
			get => _state;
			set {
				// When we change the state of the card... fire an event
				OnStateChanged(_state, value);
				_state = value;
			}
		}

		

		public bool IsOurTurn => CardGameManager.instance.IsPlayerTurn;
		public bool OwnedByPlayer => cardOwner == 0; // TODO: If we change the definition of who the player is, make sure to update this property!

		// Index representing which player or AI owns this card
		public int cardOwner;

		[SerializeField] private string _name;
		private string nameCache = null;
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
		
		[SerializeField] private string _cost;
		private string costCache = null;
		public string cost {
			set {
				_cost = value;
				costCache = null;
			} 
			get {
				costCache ??= modifications.Aggregate(_cost, (current, mod) => mod.GetCost(current));
				return costCache;
			}
		}
		
		[SerializeField] private Sprite _art;
		private Sprite artCache = null;
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
		
		[SerializeField] private string _rules;
		private string rulesCache = null;
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

		[SerializeField] private PropertyDictionary _properties;
		private PropertyDictionary propertiesCache = null;
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

		public void AddModification(Modification mod, int? index = null) {
			if(index is null) modifications.Add(mod);
			else modifications.Insert(index.Value >= 0 ? index.Value : index.Value + modifications.Count, mod);

			// Since a new modification has been added, invalidate the caches
			InvalidateCaches();
		}

		// Function which goes through all of the attributes which can be modified and invalidates their caches 
		// TODO: Add a way to invalidate the cache from the Unity UI
		public virtual void InvalidateCaches() {
			nameCache = null;
			costCache = null;
			artCache = null;
			rulesCache = null;
			propertiesCache = null;
		}
		
		
		// Helper function which sends the card to its owner's graveyard!
		public void SendToGraveyard() {
			if (OwnedByPlayer) {
				container.SendToContainer(this, CardGameManager.instance.playerGraveyard);
			} else {
				// TODO: Add back to the associated monster deck and shuffle it	
			}
		}
		

		// When a new card is enabled, add it to the list of cards... when it is disabled remove it from the list of cards 
		// This list of cards acts as a cache so that all of the cards currently in the scene can be found quickly!
		public static readonly List<CardBase> ActiveCards = new();
		public void OnEnable() => ActiveCards.Add(this);
		public void OnDisable() => ActiveCards.Remove(this);
		

		public void Start() {
			if(renderer is not null) renderer.card = this;
		}


		// Called when a turn starts
		public virtual void OnTurnStart() {}
		// Called when a turn ends (processes modifications lifetimes)
		// NOTE: needs to be called in overriding classes if automatic modifier removal is expected to function!
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
		// Called whenever the state changes
		public virtual void OnStateChanged(State oldState, State newState) { }
		// Called when a monster reveals this card // TODO: Implement
		public virtual void OnMonsterReveal() { }
		// Called when the player begins dragging this card
		public virtual void OnDragged() { }
		// Called when this card is played to the field
		public virtual void OnPlayed() { }
		// Called when this card targets something
		public virtual void OnTarget(CardBase target) { }
		// Called when this card is targeted by something
		public virtual void OnTargeted(CardBase targeter) {}
	}
}