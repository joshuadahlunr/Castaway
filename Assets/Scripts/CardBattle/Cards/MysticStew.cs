using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Restore x HP to the ship
    /// </summary>
    /// <author> Jared White </author>
    public class MysticStew : Card.ActionCardBase {
        /// <summary>
        /// Can't target anything
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// Apply healing to the player
        /// </summary>
        public override void OnTarget(Card.CardBase _) {
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyHealing(properties["primary"]);
                SendToGraveyard();
        }
    }   
}