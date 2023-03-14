using System;
using System.Collections;
using UnityEngine;

namespace CardBattle.Card {
	/// <summary>
	///     Base class for status cards
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class StatusCardBase : ActionCardBase {
		/// <summary>
		///     Override of OnStateChanged that automatically calls OnDrawn when the status is Drawn
		/// </summary>
		public override void OnStateChanged(State oldState, State newState) {
			// Check if the card was just moved to the "in hand" state
			if ((oldState & State.InHand) == 0 && (newState & State.InHand) > 0)
				// Call the OnDrawn() method
				OnDrawn();
		}

		/// <summary>
		///     Helper function which draws a new card and then sends this card to the graveyard
		/// </summary>
		public void DrawAndGrave() {
			// Draw a new card
			Draw();
			// Send this card to the graveyard
			SendToGraveyard();
		}

		/// <summary>
		///     Helper function which performs an action after scaling this card up to indicate stuff is about to happen!
		/// </summary>
		/// <param name="toPerform">The action to perform after the animation</param>
		/// <returns>Enumerable which is expected to be started as a coroutine. See: <see cref="MonoBehaviour.StartCoroutine(IEnumerable)" /></returns>
		public IEnumerator IndicationAnimation(Action toPerform) {
			// Set the initial time to zero
			float time = 0;
			// Get the initial scale of the card
			var initialScale = transform.localScale;
			// Scale up the card for two seconds
			while (time < 2) {
				time += Time.deltaTime;
				transform.localScale = initialScale * (1 + time / 2);
				yield return null;
			}

			// Call the specified action after the animation
			toPerform();
		}

		/// <summary>
		///     Callback called whenever this status is drawn
		/// </summary>
		public virtual void OnDrawn() { }
	}
}