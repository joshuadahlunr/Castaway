using CardBattle.Card;
using CardBattle.Containers;
using System.Collections.Generic;
using System.Linq;

namespace CardBattle
{
    /// <summary>
    ///     Electric status effect, which does damage to
    ///     everything, then is removed from the game
    /// </summary>
    /// <author> Misha Desear </author>
    public class ElectricStatus : Card.StatusCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;
                
        /// <summary>
        /// Affects everything in play on the board, including the player
        /// </summary>
        public override void OnDrawn() {
            /// <summary>
            /// Apply damage to the player
            /// </summary>
            CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
            var monsters = CardGameManager.instance.monsters;

            /// <summary>
            /// Apply damage to every monster card in play
            /// </summary>
            for (int i = 0; i < monsters.Count(); i++)
            {
                DamageTargetOrPlayer(properties["primary"], monsters[i]);
            }

            CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
        }

        public override void OnMonsterReveal()
        {
            OnDrawn();
        }

        public override void OnTarget(Card.CardBase _)
		{
			if (!OwnedByPlayer && CardGameManager.instance.IsPlayerTurn == false)
			{
				RemoveFromGame();
			}
		}
    }
}


