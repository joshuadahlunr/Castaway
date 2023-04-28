using CardBattle.Card;

namespace CardBattle {
	/// <summary>
	///     Class for the basic Attack card
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Attack : ActionCardBase {
		// Can only target monsters and equipment
		public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Monster |
			     CardFilterer.CardFilters.InPlay | CardFilterer.CardFilters.Equipment);

		// Can target monsters in addition to everything allowed by TargetingFilters
		public override CardFilterer.CardFilters MonsterTargetingFilters
			=> TargetingFilters | CardFilterer.CardFilters.Monster;

		/// <summary>
		///     Called when the player targets something with this card
		/// </summary>
		/// <param name="_target">The target card</param>
		public override void OnTarget(CardBase _target) {
			// Convert target to HealthCardBase
			var target = _target?.GetComponent<HealthCardBase>();
			// Return if the target is null or owned by the player
			if (NullAndPlayerCheck(target)) return;

			AudioManager.instance.soundFXPlayer.PlayTrackImmediate("Attack");

			// Damage target (falling back to player if we are monster and not targeting anything!)
			DamageTargetOrPlayer(properties["primary"], target);

			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}