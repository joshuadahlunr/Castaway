using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Card;
using Extensions;
using UnityEngine;

public class CardContainerBase : MonoBehaviour, IEnumerable<CardBase> {
	public enum Facing {
		FaceUp,
		FaceDown
	}
	public virtual Facing facing => Facing.FaceUp;

	[SerializeField] protected /*readonly*/ List<CardBase> cards = new();

	public int Count => cards.Count;

	public CardBase this[int i] {
		get => cards[i];
		set => cards[i] = value;
	}

	public CardBase this[string name] {
		get => cards.FirstOrDefault(card => card.name == name);
		set {
			var pair = cards.WithIndex().FirstOrDefault(card => card.item.name == name);
			if (pair.item is null) throw new ArgumentOutOfRangeException($"The card {name} could not be found in the container");
			cards[pair.index] = value;
		}
	}

	public int Index(Card.CardBase card) => cards.WithIndex().FirstOrDefault(pair => pair.item == card).index;
	public int Index(string name) => cards.WithIndex().FirstOrDefault(pair => pair.item.name == name).index;

	public IEnumerator<CardBase> GetEnumerator() => cards.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
	public virtual void AddCard(Card.CardBase card, int index = -1) {
		card.container = this;
		
		if(index >= 0) cards.Insert(index, card);
		else cards.Add(card);
		
		// Make sure the newly added card is a child of this container
		card.transform.parent = transform;
		if(index >= 0) card.transform.SetSiblingIndex(index);
		
		// Make the card active or inactive depending on if this container is face up or face down
		// TODO: Does deactivating the cards (update not running...) have any negative ramifications?
		card.gameObject.SetActive(facing == Facing.FaceUp);
	}

	public virtual void RemoveCard(int index) {
		cards[index].container = null;
		cards.RemoveAt(index);
	}
	public void RemoveCard(string name) => RemoveCard(Index(name));
	public void RemoveCard(Card.CardBase card) => RemoveCard(Index(card));
	

	public virtual void SendToContainer(CardContainerBase newContainer, int index) {
		var card = cards[index];
		RemoveCard(card);
		newContainer.AddCard(card);
	}
	public void SendToContainer(CardContainerBase newContainer, string name) => SendToContainer(newContainer, Index(name));
	public void SendToContainer(CardContainerBase newContainer, Card.CardBase card) => SendToContainer(newContainer, Index(card));
	
	public virtual void Swap(int A, int B) {
		// Swap the cards
		(cards[A], cards[B]) = (cards[B], cards[A]);
		// Swap them in the child hierarchy
		cards[A].transform.SetSiblingIndex(B);
		cards[B].transform.SetSiblingIndex(A);
	}
}