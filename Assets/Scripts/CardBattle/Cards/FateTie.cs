using CardBattle.Card;

namespace CardBattle {
	/// <summary>
	///     Class for the FateTie card (this card links the player's health with a monster and makes it so they both take the same damage)
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class FateTie : ActionCardBase {
		// Can only target monsters and equipment
		public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Monster |
			     CardFilterer.CardFilters.InPlay /*| CardFilterer.CardFilters.Equipment*/);

		// Can target monsters in addition to everything allowed by TargetingFilters
		public override CardFilterer.CardFilters MonsterTargetingFilters
			=> TargetingFilters | CardFilterer.CardFilters.Monster;

		/// <summary>
		///     Called when the player targets something with this card create a new FateTie passive which links the player and monster's health bars
		/// </summary>
		/// <param name="_target">The target card</param>
		public override void OnTarget(CardBase _target) {
			// Convert target to HealthCardBase
			var target = _target?.GetComponent<MonsterCardBase>();
			if (NullAndPlayerCheck(target)) return;

			var passive = CardGameManager.instance.gameObject.AddComponent<CardBattle.Passives.FateTie>();
			passive.Setup(OwnedByPlayer ? target : OwningMonster);

			SendToGraveyard();
		}
	}
}