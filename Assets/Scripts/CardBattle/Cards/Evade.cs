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
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters =>
            CardFilterer.CardFilters.All;

        public override void OnTarget(Card.CardBase _)
        {
            var mod = new ReductionModification {
                turnsRemaining = properties["duration"],
                DamageReductionAmount = properties["reduction"]
            };

            // If owned by the player, apply the damage 
            if (OwnedByPlayer)
            {
                for (int i = 0; i < CardGameManager.instance.monsters.Length; i++)
                {
                    CardGameManager.instance.monsters[i].AddModification(mod);
                }
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