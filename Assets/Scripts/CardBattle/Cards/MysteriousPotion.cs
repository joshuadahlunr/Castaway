using UnityEngine;
using System.Collections;
using CardBattle.Card;

namespace CardBattle
{
    public class MysteriousPotion : Card.ActionCardBase
    {
        // Can only target monsters
        public override CardFilterer.CardFilters TargetingFilters => 
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        /// <summary>
        /// When the player targets a monster, add a poison card to their deck and send to graveyard
        /// <summary>
        public override void OnTarget(CardBase _target)
        {
            if (OwnedByPlayer)
            {
                var target = _target?.GetComponent<Card.MonsterCardBase>();
                if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

                target.deck.cardDB.Instantiate("Poison"); 
            }

            else 
            {
                CardGameManager.instance.playerDeck.cardDB.Instantiate("Poison");
            }

                SendToGraveyard();
        }
    }
}