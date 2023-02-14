using System.Linq;

namespace CardBattle
{
    /// <summary>
    ///     Electric status effect, which does damage to 
    ///     everything, then is removed from the game
    ///     <author> Misha Desear </author>
    /// </summary>
    public class ElectricStatus : Card.StatusCardBase
    {
        // Affects everything on the board, so don't filter out anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.None;
        
        public override bool CanTargetPlayer => false;

        public override void OnDrawn() => StartCoroutine(
            IndicationAnimation(()=> {
                // TODO: do set amount of damage to every card on the board

                RemoveFromGame();
        }));
    }
}
