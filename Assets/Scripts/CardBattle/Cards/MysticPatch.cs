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
            // If owned by the player, iterate through our deck 
            // and add to the counter as we remove status cards
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
                
                // Apply healing based on amount of cards removed
                CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.ApplyHealing(removalCounter);
                NotificationHolder.instance.CreateNotification("Removed " + removalCounter.ToString() + " status effect(s) from deck!");
            }

            else
            {
                // Otherwise, do the same, but remove cards from 
                // monster deck and restore health to the monster
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