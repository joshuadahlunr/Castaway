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
        // Can't target anything 
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;
        public override void OnDrawn() 
        {
            // Apply 1 damage to player
            if (OwnedByPlayer)
            {
                CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
            }
                 
                // Send to Graveyard
            SendToGraveyard();
        }

        public override void OnMonsterReveal()
        {
            DamageTargetOrPlayer(properties["primary"], OwningMonster);
        }

        public override void OnTarget(Card.CardBase _)
		{
            SendToGraveyard();
		}
    }
}