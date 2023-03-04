using UnityEngine;

namespace CardBattle {

    /// <summary>
    /// Squawking card
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Squawk : Card.ActionCardBase {
        public override bool CanTargetPlayer => false;
        public AudioSource squawk;

        public override void OnTarget(Card.CardBase _) {
            squawk.Play();
            Debug.Log("Squawk");
            SendToGraveyard();
        }
    }
}