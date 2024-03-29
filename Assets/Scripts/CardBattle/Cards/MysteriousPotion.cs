using UnityEngine;
using System.Collections;
using CardBattle.Card;

namespace CardBattle
{
    /// <summary>
    ///     Mysterious Potion: instantiates one Poison card
    ///     into the target's deck
    /// </summary>
    /// <author> Misha Desear </author>
    public class MysteriousPotion : Card.ActionCardBase
    {
        // Can only target monsters
        public override CardFilterer.CardFilters TargetingFilters => 
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        [SerializeField] private Card.StatusCardBase poison;

        /// <summary>
        /// When the player targets a monster, add a poison card to their deck and send to graveyard
        /// <summary>
        public override void OnTarget(CardBase _target)
        {
            if (OwnedByPlayer)
            {
                var target = _target?.GetComponent<Card.MonsterCardBase>();
                if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

                target.deck.AddCard(Instantiate(poison)); 
            }

            else 
            {
                CardGameManager.instance.playerDeck.AddCard(Instantiate(poison));
            }

                SendToGraveyard();
        }
    }
}