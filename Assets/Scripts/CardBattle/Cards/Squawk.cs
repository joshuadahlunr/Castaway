using CardBattle.Card;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    ///     Squawking card
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Squawk : ActionCardBase {
		// Override the CanTargetPlayer property to always return false
		public override bool CanTargetPlayer => false;


        /// <summary>
        ///     Plays the squawk audio clip, logs a message to the console, and sends the card to the graveyard.
        /// </summary>
        /// <param name="_">Unused.</param>
        public override void OnTarget(CardBase _) {
			// Play the squawk audio clip
			AudioManager.instance.soundFXPlayer.PlayTrackImmediate("Squawk");

			// Log a message to the console
			Debug.Log("Squawk");

			// Send the card to the graveyard
			SendToGraveyard();
		}
	}
}