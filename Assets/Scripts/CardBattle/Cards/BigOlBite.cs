using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardBattle.Containers;

namespace CardBattle {
    /// <summary>
    /// Big ol Bite attack: takes away card from deck and does a large amount damage
    /// </summary>
    /// <author>
    /// Dana Conley
    /// </author>
    public class BigOlBite : Card.ActionCardBase {
        // Can target player and equipment
        //public override CardFilterer.CardFilters TargetingFilters
        //    => ~(CardFilterer.CardFilters.Player |
        //         CardFilterer.CardFilters.InPlay);
        //public override CardFilterer.CardFilters PlayerTargetingFilters
        //    => TargetingFilters | CardFilterer.CardFilters.Player;

        // Can target anything but monsters
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.Monster;
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.Monster;
        public override bool CanTargetPlayer => false;

        /// Damages target
        public override void OnTarget(Card.CardBase _target) {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            // Target cannot be null
            if (target is null)
            {
                RefundAndReset();
                return;
            }

            DamageTargetOrPlayer(properties["primary"], target);

            SendToGraveyard();
        }
    }
}