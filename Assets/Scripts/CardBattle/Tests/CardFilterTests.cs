using System.Collections;
using CardBattle;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>Unit Tests for Card Filter</summary>
/// <author>Dana Conley</author>

public class CardFiltererTests
{
    [Test]
    public void FilterCards_FiltersOutEnemyCards()
    {
        var card1 = new Card.CardBase() { OwnedByPlayer = true };
        var card2 = new Card.CardBase() { OwnedByPlayer = false };
        var cards = new List<Card.CardBase>() { card1, card2 };
        var result = CardFilterer.FilterCards(CardFilterer.CardFilters.Enemy, cards);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(card1, result[0]);
    }
}