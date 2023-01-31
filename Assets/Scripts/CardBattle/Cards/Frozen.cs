
namespace CardBattle {

	/// <summary>
	/// Basic prototype attack card used for testing
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class FrozenStatus : Card.StatusCardBase {
		// Can't target anything (filter out everything)!
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
		public override bool CanTargetPlayer => false;

		public override void OnDrawn() => StartCoroutine(
			IndicationAnimation(() => {
				// TODO: We should show the player some kind of indication that this is happening!
				CardGameManager.instance.playerHand.SendAllToContainer(CardGameManager.instance.playerGraveyard);

				// Max out the player's damage negation
				CardGameManager.instance.playerHealthState = CardGameManager.instance.playerHealthState.SetTemporaryDamageReduction(int.MaxValue);

				// Remove the card from the game!
				RemoveFromGame();
			}));
	}

}
