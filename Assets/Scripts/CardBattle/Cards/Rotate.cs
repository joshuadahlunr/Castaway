using System.Collections;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Card which rotates the ship some amount
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Rotate : Card.ActionCardBase {
        // The prefab that will be instantiated to rotate the ship
        public GameObject rotatorPrefab;

        // This card can target any object
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        // This card cannot target players directly
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// Called when the player targets something with this card. Rotates the ship and sends the card to the graveyard.
        /// </summary>
        /// <param name="_">The card that was targeted. Not used in this method.</param>
        public override void OnTarget(Card.CardBase _) {
            // Log a message to the console
            Debug.Log("Rotate logic goes here...");

            // Start a coroutine to rotate the ship on the next frame
            StartCoroutine(RotateNextFrame());

            // Define the coroutine
            IEnumerator RotateNextFrame() {
                // Wait for one frame
                yield return null;

                // If the card is owned by the player...
                if (OwnedByPlayer) {
                    // Instantiate the rotatorPrefab as a child of the ship object
                    Instantiate(rotatorPrefab, CardGameManager.instance.ship.transform);
                    // Log a message to the console
                    Debug.Log("Created rotator!");
                }
                // If the card is not owned by the player...
                else {
                    // Choose a random angle for the ship rotation
                    var angle = Mathf.Round(Random.Range(0f, 360f) / 30) * 30;
                    // Set the rotation of the ship object
                    CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(0, angle, 0);
                }

                // Send the card to the graveyard
                SendToGraveyard();
            }
        }
    }
}
