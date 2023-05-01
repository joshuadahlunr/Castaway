using System;
using CardBattle.Card;
using JetBrains.Annotations;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

/// <summary>
///     Scriptable Object holding references to a bunch of monsters
/// </summary>
/// <author>Joshua Dahl</author>
[CreateAssetMenu(fileName = "MonsterDatabase", menuName = "ScriptableObjects/MonsterDatabase")]
public class MonsterDatabase : ScriptableObject {
	[Serializable]
	public struct MonsterData {
		public MonsterCardBase monsterCard;
		public CardBase[] deckList;
		[CanBeNull] public string deckListString;

		public MonsterCardBase PopulateDeck(MonsterCardBase instantiatedMonsterToPopulate) {
			if (!string.IsNullOrEmpty(deckListString))
				instantiatedMonsterToPopulate.deck.DatabaseLoad(deckListString);

			if (deckList is null) return instantiatedMonsterToPopulate;
			foreach (var card in deckList)
				instantiatedMonsterToPopulate.deck
					.AddCard(Instantiate(card)); // We instantiate the card here to allow prefabs to be passed in (non prefabs should just get copied)

			return instantiatedMonsterToPopulate;
		}
	}

	/// <summary>
	///     Serializable dictionary used in the database
	/// </summary>
	[Serializable]
	public class MonsterDictionary : SerializableDictionaryBase<string, MonsterData> { }

	/// <summary>
	///     Database mapping from monster names to monster cards and the cards in their decks
	/// </summary>
	public MonsterDictionary cards = new();

	// Instantiate a card in the database and return a reference to its CardBase
	public MonsterCardBase Instantiate(string cardName) => cards.TryGetValue(cardName, out var m)
		? m.PopulateDeck(Instantiate(m.monsterCard).GetComponent<MonsterCardBase>()) : null;

	public MonsterCardBase Instantiate(string cardName, Transform parent) => cards.TryGetValue(cardName, out var m)
		? m.PopulateDeck(Instantiate(m.monsterCard, parent).GetComponent<MonsterCardBase>()) : null;

	public MonsterCardBase Instantiate(string cardName, Vector3 position, Quaternion rotation)
		=> cards.TryGetValue(cardName, out var m)
			? m.PopulateDeck(Instantiate(m.monsterCard, position, rotation).GetComponent<MonsterCardBase>()) : null;

	public MonsterCardBase Instantiate(string cardName, Vector3 position, Quaternion rotation, Transform parent)
		=> cards.TryGetValue(cardName, out var m) 
			? m.PopulateDeck(Instantiate(m.monsterCard, position, rotation, parent).GetComponent<MonsterCardBase>()) : null;
}