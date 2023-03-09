using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Add 1 HP to the ship
    /// </summary>
    /// <author> Jared White </author>
    public class BailWater : Card.ActionCardBase {
        /// <summary>
        /// Can't target anything
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// Apply healing to the player
        /// </summary>
        public override void OnTarget(Card.CardBase _) {
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyHealing(1);
                SendToGraveyard();
        }
    }   
}