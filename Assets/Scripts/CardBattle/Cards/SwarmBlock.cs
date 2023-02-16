using System;

namespace CardBattle {
    /// <summary>
    /// This card prevents damage from Swarm Types
    /// </summary>
    /// <author>Dana Conley</author>
    public class SwarmBlock : Card.ActionCardBase {
        // Targets Swarm Type monsters
        public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);
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

                // If the target isn't a Swarm Type then return to hand
                ///
                /// TO DO: implement check for monster type (once monsters have this feature implemented)
                ///
                
            // Card prevents damage from this monster
            /// TO DO: implement this...
            /// (Still figuring out the set up of your code)
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
