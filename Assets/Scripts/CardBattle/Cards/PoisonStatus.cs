using CardBattle.Card;
using CardBattle.Containers;
using System.Collections.Generic;
using System.Linq;

namespace CardBattle {
    /// <summary>
    ///     Poison Status Effect 
    ///     Causes 1 damage on draw, does not affect equipment
    ///     <author> Jared White </author>
    /// </summary>
    public class PoisonStatus : Card.StatusCardBase {
        // Can't target equipment 
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.EquipmentImpl;
        public override bool CanTargetPlayer => false;
        public override void OnDrawn() => StartCoroutine(
            IndicationAnimation(() => {
                // Apply 1 damage to player
                if (OwnedByPlayer)
                {
                    CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
                }
                else 
                {
                    OwningMonster.healthState.ApplyDamage(properties["primary"]);
                }
                // Send to Graveyard
                SendToGraveyard();
            }));

    }
}