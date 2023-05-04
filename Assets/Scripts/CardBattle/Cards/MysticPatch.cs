using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Mystic Patch card, which iterates through owner's entire deck and removes
    /// all status effect cards, then applies healing equal to the number of status
    /// effects removed from deck
    /// </summary>
    /// <author> Misha Desear </author>
    public class MysticPatch : ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        private int removalCounter = 0;

        public override void OnTarget(Card.CardBase _)
        {

            if (OwnedByPlayer)
            {
                var playerDeck = CardGameManager.instance.playerDeck;
                for (int i = 0; i < playerDeck.Count; i++)
                {
                    if (playerDeck[i] is Card.StatusCardBase)
                    {
                        playerDeck.RemoveCard(i);
                        removalCounter++;
                    }
                }
                
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyHealing(removalCounter);
                NotificationHolder.instance.CreateNotification("Removed " + removalCounter.ToString() + " status effect(s) from deck!");
            }

            else
            {
                var monsterDeck = OwningMonster.deck;
                for (int i = 0; i < monsterDeck.Count; i++)
                {
                    if (monsterDeck[i] is Card.StatusCardBase)
                    {
                        monsterDeck.RemoveCard(i);
                        removalCounter++;
                    }
                }

                OwningMonster.healthState = OwningMonster.healthState.ApplyHealing(removalCounter);
            }

            SendToGraveyard();
        }
    }
}