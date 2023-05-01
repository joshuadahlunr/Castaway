using CardBattle.Card;
using UnityEngine;

namespace CardBattle 
{
    /// <summary>
    /// Evade card, which reduces the amount of damage taken from the next attack
    //  performed by the targetted monster (or player, if owned by a monster)
    /// </summary>
    /// <author> Misha Desear </author>
    public class Blunderbuss : Card.ActionCardBase
    {
        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);

        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        public override void OnTarget(Card.CardBase _target)
        {
            var jamChance = Random.Range(1, 3); // 33% chance to jam
            if (jamChance == 1)
            {
                NotificationHolder.instance.CreateNotification("The blunderbuss jammed!");
                SendToGraveyard();
            }
            else
            {
                var target = _target?.GetComponent<Card.HealthCardBase>();
                if (NullAndPlayerCheck(target))
                {  
                    RefundAndReset();
                    return;
                }
                DamageTargetOrPlayer(properties["primary"], target);
            }

            SendToGraveyard();
        }
    }
}
