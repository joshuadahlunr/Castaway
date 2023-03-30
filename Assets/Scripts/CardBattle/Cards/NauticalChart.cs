using UnityEngine;
using CardBattle.Card;

namespace CardBattle {
    /// <summary>
    /// Nautical Chart: Reveals the top card of the monster's deck, this card is removed from the game after
    /// </summary>
    /// <author> Jared White </author>
    public class NauticalChart : Card.ActionCardBase {
        // Can only target monster cards
        public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        public override void OnTarget(Card.CardBase _target) { 
            // If the target isn't a monster then return to hand
            var target = _target?.GetComponent<MonsterCardBase>();

            // Reveal the top card of the monster's deck!
            target.deck.RevealCard();

            RemoveFromGame();
        }
    }
}