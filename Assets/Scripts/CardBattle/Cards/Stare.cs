using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Stare Card: Makes the player inable to act for one turn
    /// <author>Jared White</author>
    /// </summary>
    public class Stare : Card.ActionCardBase {

        // Variable for the unplayable card
        public CardBase card;

        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        
        // On target, iterate through the player's deck and add Max to the cost of Rotate for x turns
        // (This should effectively disable the ability to rotate, unless there's a better way to do this...)
        public override void OnTarget(Card.CardBase _) {
            var playerHand = CardGameManager.instance.playerHand;
            var playerTurn = CardGameManager.instance.IsPlayerTurn;
            if (playerTurn == true) {
                for (int i = 0; i < playerHand.Count; i++) {
                    playerHand[i].SendToGraveyard();
                    //playerHand.cardDB.AddCard(unplayableCard, i);
                    //playerHand.AddCard(card, i);
                }
            }

            SendToGraveyard();
        }
    }
}
