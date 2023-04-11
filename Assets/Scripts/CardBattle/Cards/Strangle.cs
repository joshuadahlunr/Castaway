namespace CardBattle {

    /// <summary>
    /// Strangle card
    /// </summary>
    /// <author>Dana Conley</author>

    public class Strangle : Card.ActionCardBase
    {

        // Can target anything but monsters
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.Monster;
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.Monster;
        public override bool CanTargetPlayer => false;


        // When played, two cards removed from the game

        public override void OnTarget(Card.CardBase target)
        {

            if (target is null)
            {
                RefundAndReset();
                return;
            }

            target.RemoveFromGame();
            SendToGraveyard();
        }
    }
}