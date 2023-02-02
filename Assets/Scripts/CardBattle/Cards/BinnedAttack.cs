namespace CardBattle {
    /// <summary>
    /// Basic prototype attack card used for testing
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class BinnedAttack : Card.ActionCardBase {
        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay /*| CardFilterer.CardFilters.Equipment*/);

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

        /// <summary>
        /// Binned attacks aren't sent to the graveyard... they are removed from the game!
        /// </summary>
        public override void SendToGraveyard() => RemoveFromGame();
    }
}
