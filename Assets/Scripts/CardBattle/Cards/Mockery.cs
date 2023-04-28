using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;
using Crew;

namespace CardBattle {
	/// <summary>
	///     Class for a card which causes the target monster/player to deal reduced damage for a turn
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Mockery : ActionCardBase {
		// Can target monsters and players
		public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);
		public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.All;
		public override bool CanTargetPlayer => true;



		/// <summary>
		///     Called when the player targets something with this card
		/// </summary>
		/// <param name="target">The target card</param>
		public override void OnTarget(CardBase target) {
			// Make sure the target isn't null if we are a player
			if (NullAndPlayerCheck(target)) return;

			// If cast by a player reduce the strength of target monster
			if (OwnedByPlayer)
				foreach (var card in target.OwningMonster.deck)
					card.AddModification(new ReductionModification {
						DamageReductionAmount = properties["strength"],
						turnsRemaining = 2 // Live for one turn cycle!
					});

			// If cast by a monster reduce the player's strength
			else foreach (var card in CardGameManager.instance.playerDeck)
				card.AddModification(new ReductionModification {
					DamageReductionAmount = properties["strength"],
					turnsRemaining = 2 // Live for one turn cycle!
				});

			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}