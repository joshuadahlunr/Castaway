namespace CardBattle {


    public class Banish : Card.ActionCardBase {
        // Can target anything but monsters!
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.Monster;
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.Monster;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// When the player targets something, remove it from the game!
        /// </summary>
        public override void OnTarget(Card.CardBase target) {
            if (target is null) {
                RefundAndReset();
                return;
            }

            target.RemoveFromGame();
            SendToGraveyard();
        }
    }
}