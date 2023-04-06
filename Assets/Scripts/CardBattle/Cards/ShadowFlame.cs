using UnityEngine;
using System.Collections;
using System.Drawing.Text;
using CardBattle.Containers;

namespace CardBattle
{
    /// <summary>
    /// Shadow Flame attack: damages the target and adds Burn card into the deck
    /// </summary>
    /// <author>
    /// Dana Conley
    /// </author>
    public class ShadowFlame : Card.ActionCardBase
    {
        // Can target monsters or equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        /// Damages target
        public override void OnTarget(Card.CardBase _target)
        {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            // Target cannot be null
            if (NullAndPlayerCheck(target)) return;

            // Damage target or player if used by monster
            DamageTargetOrPlayer(properties["primary"], target);

            // Adds Burn to deck
            CardGameManager.instance.playerDeck.cardDB.Instantiate("Burn");

            SendToGraveyard();
        }
    }
}