using CardBattle.Card;
using UnityEngine;

namespace CardBattle {
	/// <summary>
	/// Card which allows the player to drag a monster around but has a chance to be destroyed
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class DamagedHarpoon : Harpoon {
		public override void OnTarget(CardBase target) {
			CardGameManager.instance.StartCoroutine(SetupHarpoon(target, point, click));

			if (Random.Range(0f, 1f) < 1 / 3f) {
				RemoveFromGame();
				return;
			}

			SendToGraveyard();
		}
	}
}