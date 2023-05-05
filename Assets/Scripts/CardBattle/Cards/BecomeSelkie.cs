using CardBattle.Card;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    ///     Card which converts a monster into its selkie form
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class BecomeSelkie : ActionCardBase {
		// Override the CanTargetPlayer property to always return false
		public override bool CanTargetPlayer => false;

        public override void OnTarget(CardBase _) {
			if (OwnedByPlayer) {
				NotificationHolder.instance.CreateNotification("Players can't take a Selkie Form!");
				RemoveFromGame();
				return;
			}

			if (OwningMonster is SelkieMonsterCardBase selkie)
				selkie.BecomeSelkie();

			SendToGraveyard();
		}
	}
}