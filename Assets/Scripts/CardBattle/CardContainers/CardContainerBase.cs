using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace CardBattle.Containers {
	/// <summary>
	/// Base type for all containers which hold cards (decks, hands, graveyards, etc...)
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class CardContainerBase : MonoBehaviour, IEnumerable<Card.CardBase> {
		/// <summary>
		/// Enum indicating if cards in the container are face up or face down
		/// When a container is FaceUp cards in it will be enabled, if it is face down they will be disabled (scripts won't run and they will be invisible)
		/// </summary>
		public enum Facing {
			FaceUp,
			FaceDown
		}

		/// <summary>
		/// Property which can be queried to check if cards in the container are face up or face down
		/// </summary>
		public virtual Facing facing => Facing.FaceUp;

		/// <summary>
		/// The list of cards being managed by this container
		/// </summary>
		[SerializeField] protected /*readonly*/ List<Card.CardBase> cards = new();

		/// <summary>
		/// Convenient access to the number of cards stored in the container
		/// </summary>
		public int Count => cards.Count;

		/// <summary>
		/// Array access operator to allow index based subscripting into the container
		/// </summary>
		/// <param name="i">Index of the card to reference</param>
		public Card.CardBase this[int i] {
			get => cards[i];
			set => cards[i] = value;
		}

		/// <summary>
		/// Array access operator to allow name based subscripting into the container
		/// </summary>
		/// <param name="i">Name of the card to reference</param>
		public Card.CardBase this[string name] {
			get => cards.FirstOrDefault(card => card.name == name);
			set {
				var pair = cards.WithIndex().FirstOrDefault(card => card.item.name == name);
				if (pair.item is null)
					throw new ArgumentOutOfRangeException($"The card {name} could not be found in the container");
				cards[pair.index] = value;
			}
		}

		/// <summary>
		/// Maps from a card reference to its index in the container
		/// </summary>
		/// <param name="card">The card to find</param>
		/// <returns>It index in the array</returns>
		/// <exception>
		///     <cref>InvalidOperationExcept</cref> if the card can not be found in the container
		/// </exception>
		public int Index(Card.CardBase card) => cards.WithIndex().First(pair => pair.item == card).index;

		/// <summary>
		/// Maps from a card name to its index in the container
		/// </summary>
		/// <param name="card">The name of the card to find</param>
		/// <returns>It index in the array</returns>
		/// <exception>
		///     <cref>InvalidOperationExcept</cref> if the card can not be found in the container
		/// </exception>
		public int Index(string name) => cards.WithIndex().First(pair => pair.item.name == name).index;

		/// <summary>
		/// Enumerator accessor, allows the container to be iterated using foreach loops
		/// </summary>
		public IEnumerator<Card.CardBase> GetEnumerator() => cards.GetEnumerator();

		/// <summary>
		/// Enumerator accessor, allows the container to be iterated using foreach loops
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Adds a card to the container and performs all of the associated book keeping
		/// </summary>
		/// <param name="card">The card to be added</param>
		/// <param name="index">Optional index indicating where it should be inserted</param>
		public virtual void AddCard(Card.CardBase card, int index = -1) {
			card.container = this;

			if (index >= 0) cards.Insert(index, card);
			else cards.Add(card);

			// Make sure the newly added card is a child of this container
			card.transform.parent = transform;
			if (index >= 0) card.transform.SetSiblingIndex(index);

			// Make the card active or inactive depending on if this container is face up or face down
			// TODO: Does deactivating the cards (update not running...) have any negative ramifications?
			card.gameObject.SetActive(facing == Facing.FaceUp);
		}

		/// <summary>
		/// Removes a card from the container and performs all of the associated book keeping
		/// </summary>
		/// <param name="index">The index of the card to remove</param>
		public virtual void RemoveCard(int index) {
			cards[index].container = null;
			cards.RemoveAt(index);
		}

		/// <summary>
		/// Removes a card from the container and performs all of the associated book keeping
		/// </summary>
		/// <param name="name">The name of the card to remove</param>
		public void RemoveCard(string name) => RemoveCard(Index(name));

		/// <summary>
		/// Removes a card from the container and performs all of the associated book keeping
		/// </summary>
		/// <param name="card">The reference to the card to remove</param>
		public void RemoveCard(Card.CardBase card) => RemoveCard(Index(card));

		/// <summary>
		/// Clears the container, removing all of the cards currently within it (and performing the associated book keeping)
		/// </summary>
		public void RemoveAllCards() {
			while (Count > 0)
				RemoveCard(0);
		}


		/// <summary>
		/// Sends a card in the container to another container (performs all of the book keeping on either end! 
		/// </summary>
		/// <param name="index">Index of the card to send</param>
		/// <param name="newContainer">The container to send it to</param>
		public virtual void SendToContainer(int index, CardContainerBase newContainer) {
			var card = cards[index];
			RemoveCard(index);
			newContainer.AddCard(card);
		}

		/// <summary>
		/// Sends a card in the container to another container (performs all of the book keeping on either end! 
		/// </summary>
		/// <param name="name">Name of the card to send</param>
		/// <param name="newContainer">The container to send it to</param>
		public void SendToContainer(string name, CardContainerBase newContainer) =>
			SendToContainer(Index(name), newContainer);

		/// <summary>
		/// Sends a card in the container to another container (performs all of the book keeping on either end! 
		/// </summary>
		/// <param name="card">Reference to the card to send</param>
		/// <param name="newContainer">The container to send it to</param>
		public void SendToContainer(Card.CardBase card, CardContainerBase newContainer) =>
			SendToContainer(Index(card), newContainer);

		/// <summary>
		/// Sends all cards in the container to another container
		/// </summary>
		/// <param name="newContainer">The container to send all of our cards to</param>
		public void SendAllToContainer(CardContainerBase newContainer) {
			while (Count > 0)
				SendToContainer(Count - 1, newContainer);
		}

		/// <summary>
		/// Swaps the position of two cards within the container
		/// </summary>
		/// <remarks>This function doesn't provide the name and reference overloads most other functions do since the combinatoric overload would be too much!
		/// Instead use the .Index method to find the index associated with your name/reference</remarks>
		/// <param name="A">Index of the first card to swap</param>
		/// <param name="B">Index of the second card to swap</param>
		public virtual void Swap(int A, int B) {
			// Swap the cards
			(cards[A], cards[B]) = (cards[B], cards[A]);
			// Swap them in the child hierarchy
			cards[A].transform.SetSiblingIndex(B);
			cards[B].transform.SetSiblingIndex(A);
		}
	}
}