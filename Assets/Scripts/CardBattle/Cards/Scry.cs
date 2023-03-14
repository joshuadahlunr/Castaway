using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;

namespace CardBattle {
    /// <summary>
    ///     Scry is a card that reveals the top card of a deck and makes it slightly less powerful in every way for a while!
    /// </summary>
    /// <remarks>
    ///     This card has two properties:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><c>duration</c> (utility): How many turns the negative effect lasts</description>
    ///         </item>
    ///         <item>
    ///             <description><c>reduction</c> (utility): How much to worsen every property of the card by</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <author>Joshua Dahl</author>
    public class Scry : ActionCardBase {
		// Can't target anything but monsters...
		public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        /// <summary>
        ///     When the player targets a monster... reveal the top card of its deck and slightly reduce its effectiveness
        /// </summary>
        /// <param name="_target">The target of the card action.</param>
        public override void OnTarget(CardBase _target) {
			// Create a modification which just reduces the effectiveness of a card slightly...
			var mod = new ReductionModification {
				turnsRemaining = properties["duration"], // Have the effect stick around long enough to be felt
				BlockReductionAmount = properties["reduction"],
				DamageReductionAmount = properties["reduction"],
				DrawReductionAmount = properties["reduction"],
				HealthReductionAmount = properties["reduction"],
				UtilityReductionAmount = properties["reduction"],
				HealthPropertiesReductionAmount = properties["reduction"]
			};

			if (OwnedByPlayer) {
				// If the target isn't a monster then return to hand
				var target = _target?.GetComponent<MonsterCardBase>();
				if (target is null) {
					RefundAndReset();
					return;
				}

				// Reveal the top card of the monster's deck!
				target.deck.RevealCard();

				// Reduce every property of the card by 1
				target.deck.revealedCards[^1].Item1.AddModification(mod);

				// For a monster this card only does something if there are cards in the player's deck!
			} else if (CardGameManager.instance.playerDeck.Count > 0) {
				var costIncrease = new CostIncreaseModification {
					turnsRemaining = properties["duration"], // Have the effect stick around long enough to be felt
					increase = new PeopleJuice.Cost { PeopleJuice.Types.Generic }
				};

				var playerDeck = CardGameManager.instance.playerDeck;
				playerDeck[0].AddModification(mod);
				playerDeck[0].AddModification(costIncrease);
			}

			SendToGraveyard();
		}
	}
}