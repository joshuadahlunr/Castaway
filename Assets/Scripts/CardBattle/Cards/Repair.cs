using UnityEngine;
using CardBattle.Card;

namespace CardBattle {
	/// <summary>
	/// Engineer card that can be used to repair equipment
	/// </summary>
	/// <author>Jared White</author>
	public class Repair : ActionCardBase {
        /// <summary>
        /// Override the TargetingFilters property to only allow targeting in-play equipment cards.
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);

        /// <summary>
        /// Override the CanTargetPlayer property to disallow targeting players directly.
        /// </summary>
        public override bool CanTargetPlayer => true;

		/// <summary>
		/// Called when the player targets something with this card
		/// </summary>
		/// <param name="_target">The target card</param>
		public override void OnTarget(CardBase _target) {

            // Check if the target is an EquipmentCardBase
			if (_target is not EquipmentCardBase target) {
				// If not, refund the card and reset the game state
				RefundAndReset();
				return;
			}

            // Convert target to HealthCardBase
			target = _target?.GetComponent<EquipmentCardBase>();
            
			// Return if the target is null or owned by the player
			if (NullAndPlayerCheck(target)) return;

			// Damage target (falling back to player if we are monster and not targeting anything!)
            target.healthState = target.healthState.ApplyHealing(properties["primary"]);

			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}