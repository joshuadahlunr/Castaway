using CardBattle.Card;
using CardBattle.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CardBattle
{
    /// <summary>
    ///     Burn status effect, which deals damage to
    ///     the player, then shuffles another burn into
    ///     the deck and draws another card
    /// </summary>
    /// <author> Misha Desear </author>
    public class BurnStatus : Card.StatusCardBase
    {
        /// <summary>
        ///     Can't target anything
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        [SerializeField] private Card.StatusCardBase burn;

        public override void OnDrawn()
        {
            /// <summary>
            ///     Apply damage to the owner and add another burn to the deck
            /// </summary>
            if (OwnedByPlayer)
            {
                CardGameManager.instance.playerHealthState.ApplyDamage(properties["primary"]);
                CardGameManager.instance.playerDeck.AddCard(Instantiate(burn));
                CardGameManager.instance.DrawPlayerCard();
            }

            SendToGraveyard();
        }

        /// <summary>
        ///     Apply damage to the monster and add burn to their deck
        /// </summary>
        public override void OnMonsterReveal()
        {
            DamageTargetOrPlayer(properties["primary"], OwningMonster);
            OwningMonster.deck.AddCard(Instantiate(burn));
        }
    }
}
