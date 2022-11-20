using UnityEngine;

namespace Card {
	public class ActionCardBase : CardBase {
		// Error whenever an action card is played instead of targeted (should never happen)
		public sealed override void OnPlayed() {
			Debug.LogError("Action cards can't be played, they must target!");
			var draggable = GetComponent<DraggableBase>();
			draggable.Reset();
		}
	}
}