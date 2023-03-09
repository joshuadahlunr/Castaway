using UnityEngine;
using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;

namespace CardBattle {
    /// <summary>
    /// Nautical Chart: Reveals the top card of the monster's deck, this card is removed from the game after
    /// </summary>
    /// <author> Jared White </author>
    public class NauticalChart : Card.ActionCardBase {
        // Can't only target monster cards
        public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        public override void OnTarget(Card.CardBase _target) { 
            var target = _target?.GetComponent<Card.MonsterCardBase>();
            target.deck.RevealCard();
            RemoveFromGame();
        }
    }
}