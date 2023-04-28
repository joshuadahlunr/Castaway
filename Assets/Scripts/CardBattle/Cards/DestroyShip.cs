using CardBattle.Card;
using Crew;

namespace CardBattle {
	/// <summary>
	///     Class for a card which causes the player to lose the game
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class DestroyShip : ActionCardBase {
		// Can target anything
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
		public override CardFilterer.CardFilters MonsterTargetingFilters => TargetingFilters;
		public override bool CanTargetPlayer => true;


		/// <summary>
		///     Called when the player targets something with this card
		/// </summary>
		/// <param name="target">The target card</param>
		public override void OnTarget(CardBase target) {
			// The player loses the game! (Deal infinite damage to the ship!)
			CardGameManager.instance.playerHealthState =
				CardGameManager.instance.playerHealthState.ApplyDamage(int.MaxValue / 2);

			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}