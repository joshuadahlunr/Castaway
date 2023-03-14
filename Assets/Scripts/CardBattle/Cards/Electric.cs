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

        /// <summary>
        /// Affects everything in play on the board, including the player
        /// </summary>
        ///

        public override bool CanTargetPlayer => false;
        public override void OnDrawn() => StartCoroutine(
            IndicationAnimation(() => {
                /// <summary>
                /// Apply damage to the player
                /// </summary>
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);

                /// <summary>
                /// Apply damage to every monster and equipment card in play
                /// </summary>
                foreach (var card in CardFilterer.FilterCards(~(CardFilterer.CardFilters.Monster
                    | CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay)))
                {
                    var cardHealth = card.GetComponent<HealthCardBase>();
                    cardHealth.healthState = cardHealth.healthState.ApplyDamage(properties["primary"]);
                }

                /// <summary>
                /// Finally, remove from the game
                /// </summary>
                ///
                RemoveFromGame();
            }));

    }
}
