using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

/// <summary>
/// Scriptable Object holding references to a bunch of cards
/// </summary>
/// <remarks>Used to convert names stored in SQL to Unity Objects</remarks>
/// <author>Joshua Dahl</author>
[CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/CardDatabase")]
public class CardDatabase : ScriptableObject {

	/// <summary>
	/// Serializable dictionary used in the database
	/// </summary>
	[Serializable]
	public class CardDictionary : SerializableDictionaryBase<string, Card.CardBase> {}
	/// <summary>
	/// Database mapping from card names to card prefabs
	/// </summary>
	public CardDictionary cards = new CardDictionary();

	// Instantiate a card in the database and return a reference to its CardBase
	public Card.CardBase Instantiate(string cardName) 
		=> cards.TryGetValue(cardName, out var card) ? Instantiate(card.gameObject).GetComponent<Card.CardBase>() : null;
	public Card.CardBase Instantiate(string cardName, Transform parent) 
		=> cards.TryGetValue(cardName, out var card) ? Instantiate(card.gameObject, parent).GetComponent<Card.CardBase>() : null;
	public Card.CardBase Instantiate(string cardName, Vector3 position, Quaternion rotation) 
		=> cards.TryGetValue(cardName, out var card) ? Instantiate(card.gameObject, position, rotation).GetComponent<Card.CardBase>() : null;
	public Card.CardBase Instantiate(string cardName, Vector3 position, Quaternion rotation, Transform parent) 
		=> cards.TryGetValue(cardName, out var card) ? Instantiate(card.gameObject, position, rotation, parent).GetComponent<Card.CardBase>() : null;
}
