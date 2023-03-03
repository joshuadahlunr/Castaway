using System.Collections;
using CardBattle;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>Unit Test for Card Filter</summary>
/// <author>Dana Conley</author>

public class CardFiltererTests {
    [Test]
    public void FilterCardsForMonsters() {
        var enemyCard = new CardBattle.Card.MonsterCardBase();
        var playerCard = new CardBattle.Card.EquipmentCardBase();
        var cards = new CardBattle.Card.CardBase[] { enemyCard, playerCard };
        var result = CardFilterer.FilterCards(CardFilterer.CardFilters.Enemy, cards);

        Assert.Contains(enemyCard, result);
    }

    [Test]
    public void FilterCardsAndDisableCards() {
        var enemyCard = new CardBattle.Card.MonsterCardBase();
        var playerCard = new CardBattle.Card.EquipmentCardBase();
        var cards = new CardBattle.Card.CardBase[] { enemyCard, playerCard };
        var result = CardFilterer.FilterAndDisableCards(CardFilterer.CardFilters.Enemy, cards);

        Assert.Contains(enemyCard, result);
    }

}
