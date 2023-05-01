using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;

namespace CardBattle 
{
    /// <summary>
    /// Evade card, which reduces the amount of damage taken from the next attack
    //  performed by the targetted monster (or player, if owned by a monster)
    /// </summary>
    /// <author> Misha Desear </author>
    public class Evade : Card.ActionCardBase
    {
        // Can only target monsters
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        public override void OnTarget(Card.CardBase _target)
        {
            var mod = new ReductionModification {
                turnsRemaining = properties["duration"],
                DamageReductionAmount = properties["reduction"]
            };

            if (OwnedByPlayer)
            {
                // If the target isn't a monster then return to hand
                var target = _target?.GetComponent<MonsterCardBase>();
                if (target is null)
                {
                    RefundAndReset();
                    return;
                }

                // Reveal top card of targeted monster's deck
                target.deck.RevealCard();

                // Apply the damage reduction modification
                target.deck.revealedCards[^1].Item1.AddModification(mod);
            }
            
            // If owned by a monster, reduce the damage done by the top card of the player's deck
            // (only if there are cards in the player's deck)
            else if (CardGameManager.instance.playerDeck.Count > 0)
            {
                var playerDeck = CardGameManager.instance.playerDeck;
                playerDeck[0].AddModification(mod);
            }

            SendToGraveyard();
        }
    }
}