using UnityEngine;
using System.Collections;
using System.Drawing.Text;
using CardBattle.Containers;

namespace CardBattle
{
    /// <summary>
    /// Big ol Bite attack: takes away card from deck and does a large amount damage
    /// </summary>
    /// <author>
    /// Dana Conley
    /// </author>
    public class BigOlBite : Card.ActionCardBase
    {
        // Can target player and equipment
        public override CardFilterer.CardFilters TargetingFilters
            => ~(CardFilterer.CardFilters.Player |
                 CardFilterer.CardFilters.InPlay);
        public override CardFilterer.CardFilters PlayerTargetingFilters
            => TargetingFilters | CardFilterer.CardFilters.Player;

        /// Damages target
        public override void OnTarget(Card.CardBase _target)
        {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            // Target cannot be null
            if (NullAndMonsterCheck(target)) return;

            DamagePlayerOrMonster(properties["primary"], target);

            SendToGraveyard();
        }
    }
}