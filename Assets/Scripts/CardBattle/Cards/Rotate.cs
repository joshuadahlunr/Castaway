using System.Runtime.CompilerServices;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    /// Card which rotates the ship some amount
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Rotate : Card.ActionCardBase {
        // Can't target anything...
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _) {
            Debug.Log("Rotate logic goes here...");

            SendToGraveyard();
        }
    }
}
