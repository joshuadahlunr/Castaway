using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    ///     Freeze card, which adds a frozen card to the player's hand
    ///     or forces the owning monster to immediately play a frozen card
    /// </summary>
    /// <author> Misha Desear </author>
    public class Freeze : Card.ActionCardBase
    {
        [SerializeField] private Card.StatusCardBase frozenCard;

        private int rotateChance;
        
        public override CardFilterer.CardFilters TargetingFilters =>
            CardFilterer.CardFilters.All;

        public override void OnTarget(Card.CardBase _)
        {
            // RNG to determine if the ship rotates after using this card
            rotateChance = Random.Range(1, 10);

            if (OwnedByPlayer)
            {
                CardGameManager.instance.playerHand.AddCard(frozenCard); // Add the frozen card to player hand
            }

            else
            {
                OwningMonster.deck.AddCard(frozenCard, 0); // Add frozen card to monster deck
                OwningMonster.deck.RevealCard(); // Reveal the frozen card
                OwningMonster.deck.PlayRevealedCard(); // Play it immediately
            }

            if (rotateChance == 1)
            {
                StartCoroutine(RotateNextFrame());

                IEnumerator RotateNextFrame()
                {
                    yield return null;
                    var angle = Mathf.Round(Random.Range(0f, 360f) / 30) * 30;
                    CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
            }
            
            SendToGraveyard();
        }
    }
}
