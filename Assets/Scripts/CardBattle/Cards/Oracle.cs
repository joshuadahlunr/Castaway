namespace CardBattle 
{
    /// <summary>
    ///     Oracle card, which draws a card when played
    /// </summary>
    /// <author>Misha Desear</author>
    public class Oracle : Card.ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;

        public override bool CanTargetPlayer => false;

        public override void OnTarget(Card.CardBase _)
        {
            if (OwnedByPlayer)
            {
                CardGameManager.instance.DrawPlayerCard();
            }
            SendToGraveyard();
        }
    }
}