using System;

namespace CardBattle {
    /// <summary>
    /// Scry is a card that reveals the top card of a deck and makes it slightly less powerful in every way for a while!
    /// </summary>
    /// <remarks>This card has two properties:
    /// 1) duration (utility): How many turns the negative effect lasts
    /// 2) reduction (utility): How much to worsen every property of the card by
    /// </remarks>
    /// <author>Joshua Dahl</author>
    public class Scry : Card.ActionCardBase {
        /// <summary>
        /// When the player targets a monster... reveal the top card of its deck and slightly reduce its effectiveness
        /// </summary>
        public override void OnTarget(Card.CardBase _target) {
            // Create a modification which just reduces the effectiveness of a card slightly...
            var mod = new Card.Modifications.Generic.ReductionModification {
                turnsRemaining = properties["duration"], // Have the effect stick around long enough to be felt
                BlockReductionAmount = properties["reduction"],
                DamageReductionAmount = properties["reduction"],
                DrawReductionAmount = properties["reduction"],
                HealthReductionAmount = properties["reduction"],
                UtilityReductionAmount = properties["reduction"],
                HealthPropertiesReductionAmount = properties["reduction"],
            }; 

            if (OwnedByPlayer) {
                // If the target isn't a monster then return to hand
                var target = _target?.GetComponent<Card.MonsterCardBase>();
                if (target is null) {
                    RefundAndReset();
                    return;
                }

                // If the target is already revealing a card... return to hand!
                if (target.deck.revealedCard != null) {
                    RefundAndReset();
                    return;
                }
                
                // Reveal the top card of the monster's deck!
                target.deck.RevealCard();

                // Reduce every property of the card by 1
                target.deck.revealedCard.AddModification(mod);
                
            // For a monster this card only does something if there are cards in the player's deck!
            } else if (CardGameManager.instance.playerDeck.Count > 0) {
                var costIncrease = new Card.Modifications.Generic.CostIncreaseModification {
                    turnsRemaining = properties["duration"], // Have the effect stick around long enough to be felt
                    increase = new PeopleJuice.Cost { PeopleJuice.Types.Generic }
                }; 
                
                var playerDeck = CardGameManager.instance.playerDeck;
                playerDeck[0].AddModification(mod);
                playerDeck[0].AddModification(costIncrease);
            }

            SendToGraveyard();
        }
    }
}
