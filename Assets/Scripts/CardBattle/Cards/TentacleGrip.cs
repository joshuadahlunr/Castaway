using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Tentacle Grip card: disables the player's ability to
    /// rotate the ship for a set duration of turns
    /// <author>Misha Desear</author>
    /// </summary>
    public class TentacleGrip : Card.ActionCardBase {

        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        
        // On target, iterate through the player's deck and add Max to the cost of Rotate for x turns
        // (This should effectively disable the ability to rotate, unless there's a better way to do this...)
        public override void OnTarget(Card.CardBase _)
        {

            var costincrease = new Card.Modifications.Generic.CostIncreaseModification
            {
                turnsRemaining = properties["duration"],
                increase = new PeopleJuice.Cost { PeopleJuice.Types.Max }
            };

            var playerDeck = CardGameManager.instance.playerDeck;

            for (int i = 0; i < playerDeck.Count; i++)
            {
                if (playerDeck[i].name == "Rotate")
                {
                    playerDeck[i].AddModification(costincrease);
                }
            }

            SendToGraveyard();
        }
    }
}
