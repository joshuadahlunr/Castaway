namespace CardBattle {


    public class Banish : Card.ActionCardBase {
        // Can target anything but monsters!
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.Monster;
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.Monster;

        /// <summary>
        /// When the player targets something, remove it from the game!
        /// </summary>
        public override void OnTarget(Card.CardBase target) {
            target.RemoveFromGame();
            SendToGraveyard();
        }
    }
}