using CardBattle.Card.Modifications.Generic;

namespace CardBattle 
{
    /// <summary>
    /// Shanty card, which applies a damage multiplier to all cards in the player's hand, 
    /// or to the top card of a monster's deck
    /// <summary>
    /// <author>Misha Desear</author>
    public class Shanty : Card.ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;

        public override bool CanTargetPlayer => false;

        public override void OnTarget(Card.CardBase _)
        {
            // Create a modification which multiplies the damage of a card
            var mod = new MultiplicationModification {
                turnsRemaining = properties["duration"], // Have the effect stick around long enough to be felt
                DamageMultiplier = properties["multiplier"]
            };

            if (OwnedByPlayer)
            {
                var playerHand = CardGameManager.instance.playerHand;
                for (int i = 0; i < playerHand.Count; i++)
                {
                    // Iterate over cards in hand and add damage multiplier
                    playerHand[i].AddModification(mod);
                }
            }
            else 
            {
                // Reveal top card of the monster's deck
                OwningMonster.deck.RevealCard();
                // Multiply the damage of that card (if applicable)
                OwningMonster.deck.revealedCards[^1].Item1.AddModification(mod);
            }
            SendToGraveyard();
        }
    }
}
