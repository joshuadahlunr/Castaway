using UnityEngine;
using System.Collections;
using System.Drawing.Text;
using CardBattle.Containers;

namespace CardBattle {
    /// <summary>
    /// Shadow Flame attack: damages the target and adds Burn card into the deck
    /// </summary>
    /// <author>
    /// Dana Conley
    /// </author>
    public class ShadowFlame : Card.ActionCardBase {
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

            // Damage target or player if used by monster
            DamageTargetOrPlayer(properties["primary"], target);

            // Adds Burn to deck
            CardGameManager.instance.playerDeck.cardDB.Instantiate("Burn");

            SendToGraveyard();
        }
    }
}