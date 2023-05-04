using CardBattle.Card;

namespace CardBattle
{
    /// <summary>
    /// Cower and Courage: sends entire hand to graveyard,
    /// then draws however many cards were discarded
    /// </summary>
    /// <author> Misha Desear </author>
    public class CowerAndCourage : ActionCardBase 
    {
        public override CardFilterer.CardFilters TargetingFilters
            => CardFilterer.CardFilters.All;
        
        private int playerHandSize = 0;

        public override void OnTarget(CardBase _) 
        {
            // If the player owns this card, dispose of entire hand
            if (OwnedByPlayer)
            {
                playerHandSize = CardGameManager.instance.playerHand.Count;
                CardGameManager.instance.playerHand.SendAllToContainer(CardGameManager.instance.playerGraveyard);
            }
        }

        public override void OnTurnEnd()
        {
            // If the player owns this card, draw however many cards were disposed of
            if (OwnedByPlayer)
            {
                for (int i = 0; i < playerHandSize; i++)
                {
                    DrawPlayerCard();
                }
            }
        }
    }
}