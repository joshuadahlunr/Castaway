using System.Collections;
using CardBattle.Card;
using CardBattle.Passives;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace CardBattle {
    /// <summary>
    /// Card which allows the player to drag a monster around
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Harpoon : ActionCardBase {
		// Can't target anything
		public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);
		public override bool CanTargetPlayer => false;

		public InputActionReference point, click;

		public override void OnTarget(CardBase target) {
			CardGameManager.instance.StartCoroutine(SetupHarpoon(target, point, click));
			SendToGraveyard();
		}
		
		public static IEnumerator SetupHarpoon(CardBase target, InputActionReference point, InputActionReference click) {
			NotificationHolder.instance.CreateNotification("Click to Place the Monster!");
			
			yield return null;
			var draggable = target.AddComponent<HarpoonDraggable>();
			draggable.TargetCurrentTransform();
			draggable.clickAction = click;
			draggable.pointerAction = point;
			draggable.enabled = false;
			yield return null;
			draggable.enabled = true;
			draggable.OnPointerDown(null);
		}
	}
}