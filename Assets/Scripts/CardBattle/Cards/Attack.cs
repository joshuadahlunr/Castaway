namespace CardBattle {

    /// <summary>
    /// Basic attack card
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Attack : Card.ActionCardBase {
        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay/*| CardFilterer.CardFilters.Equipment*/);
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _target) {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            SendToGraveyard();
        }
    }
}