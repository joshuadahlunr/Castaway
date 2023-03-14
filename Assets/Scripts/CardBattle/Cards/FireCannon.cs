// Import the Card class from the CardBattle namespace
using CardBattle.Card;

// Define the FireCannon class within the CardBattle namespace
namespace CardBattle {

    /// <summary>
    /// FireCannon is a subclass of ActionCardBase that applies damage to MonsterCardBase objects
    /// colliding with an EquipmentCardBase target.
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class FireCannon : Card.ActionCardBase {

        /// <summary>
        /// Override the TargetingFilters property to only allow targeting in-play equipment cards.
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);

        /// <summary>
        /// Override the CanTargetPlayer property to disallow targeting players directly.
        /// </summary>
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// Override the OnTarget method to apply damage to any MonsterCardBase objects colliding with the target EquipmentCardBase.
        /// </summary>
        /// <param name="_target">The target card to apply the FireCannon effect to.</param>
        public override void OnTarget(Card.CardBase _target) {
            // Check if the target is an EquipmentCardBase
            if (_target is not EquipmentCardBase target) {
                // If not, refund the card and reset the game state
                RefundAndReset();
                return;
            }

            // Get all MonsterCardBase objects colliding with the target EquipmentCardBase
            foreach (var monster in target.GetCollidingCards<MonsterCardBase>()) {
                // Apply damage to each MonsterCardBase object
                monster.healthState = monster.healthState.ApplyDamage(properties["primary"]);
            }

            // Send the FireCannon card to the graveyard to remove it from play
            SendToGraveyard();
        }
    }
}
