using System;
using System.Collections;
using UnityEngine;

namespace Card {
    /// <summary>
    ///     Base class for status cards
    /// </summary>
    public class StatusCardBase : ActionCardBase {
        /// <summary>
        ///     Override of OnStateChanged that automatically calls OnDrawn when the status is Drawn
        /// </summary>
        public override void OnStateChanged(State oldState, State newState) {
			if ((oldState & State.InHand) == 0 && (newState & State.InHand) > 0) // We weren't in the hand but now are in the hand
				OnDrawn();
		}

        /// <summary>
        ///     Helper function which draws a new card and then sends this card to the graveyard
        /// </summary>
        public void DrawAndGrave() {
			Draw();
			SendToGraveyard();
		}

        /// <summary>
        /// Helper function which performs an action after scaling this card up to indicate stuff is about to happen!
        /// </summary>
        /// <param name="toPerform">The action to perform after the animation</param>
        /// <returns>Enumerable which is expected to be started as a coroutine. See: <see cref="MonoBehaviour.StartCoroutine(IEnumerable)"/></returns>
        public IEnumerator IndicationAnimation(Action toPerform) {
	        float time = 0;
	        var initialScale = transform.localScale;
	        while (time < 2) {
		        time += Time.deltaTime;
		        transform.localScale = initialScale * (1 + time / 2);
		        yield return null;
	        }

	        toPerform();
        }

        /// <summary>
        /// Callback called whenever this status is drawn
        /// </summary>
		public virtual void OnDrawn() { }
	}
}