using System.Collections;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Card which rotates the ship some amount
    /// WIP
    /// </summary>
    /// <author>Joshua Dahl & Jared White</author>
    public class ArcaneTwirl : Card.ActionCardBase {
        public GameObject rotatorPrefab;
        
        // Can't target anything!
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _) {
            Debug.Log("Rotate logic goes here...");
            StartCoroutine(RotateNextFrame());
            
            IEnumerator RotateNextFrame() {
                yield return null;
                if (OwnedByPlayer) {
                    Instantiate(rotatorPrefab, CardGameManager.instance.ship.transform);
                    Debug.Log("Created rotator!");
                } else {
                    var angle = Mathf.Round(Random.Range(0f, 360f) / 30) * 30;
                    CardGameManager.instance.ship.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
                
                SendToGraveyard();
            }
        }
    }
}
