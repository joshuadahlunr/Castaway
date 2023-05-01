using System.Collections;
using System.Drawing.Text;
using CardBattle.Containers;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Second Row of Teeth attack: increases amount of damage
    /// </summary>
    /// <author>
    /// Dana Conley
    /// </author>
    public class SecondRowOfTeeth : Card.ActionCardBase {
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

            // Increases damage by 1
            int damage = properties["primary"];
            damage += 1;
            DamageTargetOrPlayer(properties["primary"], target);

            SendToGraveyard();
        }
    }
}