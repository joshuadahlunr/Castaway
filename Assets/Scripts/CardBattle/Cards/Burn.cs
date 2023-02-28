using CardBattle.Card;
using CardBattle.Containers;
using System.Collections.Generic;
using System.Linq;

namespace CardBattle
{
    /// <summary>
    ///     Burn status effect, which deals damage to
    ///     the player, then shuffles another burn into
    ///     the deck and draws another card
    ///     <author> Misha Desear </author>
    /// </summary>
    public class BurnStatus : Card.StatusCardBase
    {
        public static CardGameManager instance;

        /// <summary>
        /// Can't target anything
        /// </summary>
        /// 
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;

        public override bool CanTargetPlayer => false;
        public override void OnDrawn() => StartCoroutine(
            IndicationAnimation(() => {
                /// <summary>
                /// Apply damage to the player
                /// </summary>
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);

                instance.InstantiateBurn();

                instance.DrawPlayerCard();

                SendToGraveyard();
            }));
    }
}
