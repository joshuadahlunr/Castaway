using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Douse card: removes a specified number of burns 
    /// and/or frozen status cards from the owner's deck
    /// <author>Misha Desear</author>
    /// </summary>
    public class Douse : ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        private int removalCounter = 0;

        public override void OnTarget(Card.CardBase _)
        {

            if (OwnedByPlayer)
            {
                var PlayerDeck = CardGameManager.instance.playerDeck;
                for (int i = 0; i < PlayerDeck.Count; i++)
                {
                    if (PlayerDeck[i].name == "Burn" || PlayerDeck[i].name == "Frozen")
                    {
                        PlayerDeck.RemoveCard(i);
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
                var MonsterDeck = OwningMonster.deck;
                for (int i = 0; i < MonsterDeck.Count; i++)
                {
                    if (MonsterDeck[i].name == "Burn" || MonsterDeck[i].name == "Frozen")
                    {
                        MonsterDeck.RemoveCard(i);
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
