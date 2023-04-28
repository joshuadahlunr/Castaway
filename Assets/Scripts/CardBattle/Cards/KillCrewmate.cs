using CardBattle.Card;
using Crew;

namespace CardBattle {
	/// <summary>
	///     Class for a card which kills a crewmate
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class KillCrewmate : ActionCardBase {
		// Can't target anything
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.None;
		public override CardFilterer.CardFilters MonsterTargetingFilters => TargetingFilters;
		public override bool CanTargetPlayer => false;


		/// <summary>
		///     Called when the player targets something with this card
		/// </summary>
		/// <param name="_target">The target card</param>
		public override void OnTarget(CardBase target) {
			if (target.associatedCrewmate == null) {
				 if (OwnedByPlayer)
					 NotificationHolder.instance?.CreateNotification("No crewmate!");
				 RefundAndReset();
				 return;
			}

			// Remove the target's crewmate from the database
			CrewManager.KillCrewmate(target.associatedCrewmate.Value);
			// Remove the target itself from the game!
			target.RemoveFromGame();

			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}