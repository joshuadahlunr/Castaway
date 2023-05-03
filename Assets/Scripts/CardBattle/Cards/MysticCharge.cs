using CardBattle.Card;
using EncounterMap;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Player automatically wins a non-boss encounter
    /// </summary>
    /// <author> Jared White </author>
    public class MysticCharge : ActionCardBase {
        public static EncounterType encounterType;
        /// <summary>
        /// Can't target anything
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// Check if the encounter is normal encounter, if yes, player can win
        /// </summary>
        public override void OnTarget(Card.CardBase _) {
            /// If the encounter is of normal type...
           if(encounterType == EncounterType.Normal) {
                // The player wins!
               foreach (var monster in CardGameManager.instance.monsters) {
                  DamageTargetOrPlayer(100, monster);
                }
                CardGameManager.instance.CheckWinLose();
            // If the encounter is a boss encounter...
            } else {
                // Remove from game as the player cannot use it
                RemoveFromGame();
                RefundAndReset();
            }
        }
    }   
}