using UnityEngine;
using System.Collections;
using CardBattle.Containers;

namespace CardBattle {
    /// <summary>
    /// Shadow Flame, which damages the target and
    /// shuffles a Burn into the deck
    /// </summary>
    /// <author> Misha Desear </author>
    public class ShadowFlame : Card.ActionCardBase {

        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay/*| CardFilterer.CardFilters.Equipment*/);
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        [SerializeField] private Card.StatusCardBase burn;

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _target)
        {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            if (OwnedByPlayer) 
            {
                CardGameManager.instance.playerDeck.AddCard(Instantiate(burn));
            }

            else
            {
                OwningMonster.deck.AddCard(Instantiate(burn));
            }

            SendToGraveyard();

        }
    }
}