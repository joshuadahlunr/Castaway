using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;

namespace CardBattle {
    /// <summary>
    /// All Out Attack card, which multiplies the player's damage by 2 but also
    /// multiplies monsters' strength by 2 for 1 turn cycle
    /// </summary>
    /// <author> Misha Desear </author>
    public class AllOutAttack : ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters 
            => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        public override void OnTarget(Card.CardBase _) {
            // Create a modification which multiplies the damage of a card
            var mod = new MultiplicationModification {
                turnsRemaining = properties["duration"], // Have the effect stick around for a single turn cycle (2)
                DamageMultiplier = properties["multiplier"]
            };

            var playerHand = CardGameManager.instance.playerHand;
            for (int i = 0; i < playerHand.Count; i++)
            {
                // Iterate over cards in hand and add damage multiplier
                playerHand[i].AddModification(mod); 
            }

            for (int i = 0; i < CardGameManager.instance.monsters.Length; i++)
            {
                // Reveal top card of each monster's deck
                CardGameManager.instance.monsters[i].deck.RevealCard();
                // Multiply the damage of that card (if applicable)
                CardGameManager.instance.monsters[i].deck.revealedCards[^1].Item1.AddModification(mod);
            }
            SendToGraveyard();
        }
    }
}