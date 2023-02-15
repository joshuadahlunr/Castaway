using System.Linq;

namespace CardBattle
{
    /// <summary>
    ///     Lightning Spear action card, which does damage to a target and then adds electric to the deck
    ///     <author>Misha Desear</author>
    /// </summary>
    public class LightningSpear : Card.ActionCardBase
    {
        public static CardGameManager instance;
        // Can only target monsters and equipment (when implemented)
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay /*| CardFilterer.CardFilters.Equipment*/);

        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        public override void OnTarget(Card.CardBase _target)
        {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            instance.InstantiateElectric();
            
            SendToGraveyard();
        }

    }
}