// Import required namespace

using CardBattle.Card;
using UnityEngine;

// Define the namespace and class for a test attack card
namespace CardBattle.TestCards {
	/// <summary>
	///     Basic attack card
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class ActionTest : ActionCardBase {
		// Set the targeting filters to none, meaning this card can target anything
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.None;
		public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.None;

		// This method is called when this card targets another card
		public override void OnTarget(CardBase target) {
			// Output the name of the target to the console
			Debug.Log($"targeting: {target.name}");
			// Send this card to the graveyard
			SendToGraveyard();
		}

		// This method is called when this card is targeted by another card
		public override void OnTargeted(CardBase targeter) {
			// Output the name of the targeter to the console
			Debug.Log($"targeted by: {targeter.name}");
		}
	}
}