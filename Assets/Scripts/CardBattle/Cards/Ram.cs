using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Ram card, which does a large amount of damage 
    /// but has a chance to damage the ship by the same amount
    /// </summary>
    /// <author> Misha Desear </author>
    public class Ram : ActionCardBase
    {
        // Can't target anything that isn't a monster or equipment
        public override CardFilterer.CardFilters TargetingFilters
            => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);
        
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        public override void OnTarget(Card.CardBase _target)
        {
            var damageChance = Random.Range(1, 4);
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            if (damageChance == 4) // 25% chance of dealing the same amount of damage to the card owner
            {
                if (OwnedByPlayer)
                {
                    {
                        CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
                        NotificationHolder.instance.CreateNotification("Your ship has been damaged by the impact!");
                    }
                }
                else
                {
                    OwningMonster.healthState.ApplyDamage(properties["primary"]);
                }
            }

            SendToGraveyard();
        }
    }
}