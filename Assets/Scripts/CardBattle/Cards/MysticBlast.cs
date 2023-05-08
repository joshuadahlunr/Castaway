using CardBattle.Card;
using UnityEngine;
using System.Collections;

namespace CardBattle
{
    /// <summary>
    ///     Mythic Blast, which does additional damage based 
    ///     on current damage to the ship or owning monster
    /// </summary>
    /// <author> Misha Desear </author>
    public class MysticBlast : ActionCardBase
    {
        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters
            => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay
                | CardFilterer.CardFilters.Equipment);
        
        // Monsters can't target other monsters
        public override CardFilterer.CardFilters MonsterTargetingFilters
            => TargetingFilters | CardFilterer.CardFilters.Monster;

        public override void OnTarget(CardBase _target) 
        {
            // Convert target to HealthCardBase
            var target = _target?.GetComponent<HealthCardBase>();

            // Return if target is null or owned by the player
            if (NullAndPlayerCheck(target)) return;

            // If owned by the player, deal damage based on damage to the ship
            if (OwnedByPlayer)
            {
                int currentShipDamage = CardGameManager.instance.playerHealthState.maxHealth - CardGameManager.instance.playerHealthState;
                DamageTargetOrPlayer(properties["primary"] + currentShipDamage, target);
            }

            // Otherwise, deal damage based on owning monster's missing health
            else
            {
                int currentMonsterDamage = OwningMonster.healthState.maxHealth - OwningMonster.healthState.health;
                DamageTargetOrPlayer(properties["primary"] + currentMonsterDamage, target);
            }

            // Send this card to graveyard
            SendToGraveyard();
        }
    }
}