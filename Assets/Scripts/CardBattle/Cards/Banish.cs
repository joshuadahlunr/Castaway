using CardBattle.Card;

namespace CardBattle {
    /// <summary>
    ///     Represents an action card that removes a selected card from the game.
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Banish : ActionCardBase {
        /// <summary>
        ///     Gets the type of cards that this action card can target.
        ///     In this case, it can target anything except for monsters.
        /// </summary>
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.Monster;

        /// <summary>
        ///     Gets the type of monsters that this action card can target.
        ///     In this case, it can only target monsters.
        /// </summary>
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.Monster;

        /// <summary>
        ///     Gets a value indicating whether the player can be targeted by this action card.
        ///     In this case, the player cannot be targeted.
        /// </summary>
        public override bool CanTargetPlayer => false;

        /// <summary>
        ///     This method is called when the player selects a target for this action card.
        ///     If the target is null, the player is refunded and the game state is reset.
        ///     Otherwise, the target is removed from the game and the action card is sent to the graveyard.
        /// </summary>
        /// <param name="target">The target card to remove from the game.</param>
        public override void OnTarget(CardBase target) {
			if (target is null) {
				RefundAndReset(); // refund the player and reset the game state
				return;
			}

			// It costs the player 1HP to cast the card!
			if (OwnedByPlayer) {
				var pHealth = CardGameManager.instance.playerHealthState;
				pHealth.health -= 1; // We set the health manually since this is a cost... and should thus bypass block
				CardGameManager.instance.playerHealthState = pHealth;
			}

			target.RemoveFromGame(); // remove the target from the game
			SendToGraveyard(); // send the action card itself to the graveyard
		}
	}
}