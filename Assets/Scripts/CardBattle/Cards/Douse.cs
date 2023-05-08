using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Douse card: removes a specified number of burns 
    /// and/or frozen status cards from the owner's deck
    /// </summary>
    /// <author>Misha Desear</author>

    public class Douse : ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        private int removalCounter = 0;

        /// <summary>
        ///     Iterate through player deck and remove Burn and Frozen cards,
        ///     break when the duration of turns has been exceeded
        /// </summary>
        public override void OnTarget(Card.CardBase _)
        {

            if (OwnedByPlayer)
            {
                var playerDeck = CardGameManager.instance.playerDeck;
                for (int i = 0; i < playerDeck.Count; i++)
                {
                    if (playerDeck[i].name == "Burn" || playerDeck[i].name == "Frozen")
                    {
                        playerDeck.RemoveCard(i);
                        removalCounter++;
                    }

                    if (removalCounter >= properties["duration"])
                    {
                        break;
                    }
                }
            }

            else
            {
                var monsterDeck = OwningMonster.deck;
                for (int i = 0; i < monsterDeck.Count; i++)
                {
                    if (monsterDeck[i].name == "Burn" || monsterDeck[i].name == "Frozen")
                    {
                        monsterDeck.RemoveCard(i);
                        removalCounter++;
                    }

                    if (removalCounter >= properties["duration"])
                    {
                        break;
                    }
                }
            }

            SendToGraveyard();
        }
    }
}
