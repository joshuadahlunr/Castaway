using CardBattle.Card;
using CardBattle.Card.Modifications.Generic;

namespace CardBattle {
	/// <summary>
	///     Basic prototype attack card used for testing
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class FrozenStatus : StatusCardBase {
		// Can't target anything (filter out everything)!
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
		public override bool CanTargetPlayer => false;

		public override void OnDrawn()
		{
			if (OwnedByPlayer) 
			{
				CardGameManager.instance.playerHand.SendAllToContainer(CardGameManager.instance.playerGraveyard);

				// Max out the player's damage negation
				CardGameManager.instance.playerHealthState =
				CardGameManager.instance.playerHealthState.SetTemporaryDamageReduction(int.MaxValue);
			}
		}

		public override void OnMonsterReveal()
		{				
			OwningMonster.healthState = OwningMonster.healthState.SetTemporaryDamageReduction(int.MaxValue);
			NotificationHolder.instance.CreateNotification(OwningMonster.name + " is frozen solid!");
		}

		public override void OnTarget(Card.CardBase _)
		{
			if (!OwnedByPlayer && CardGameManager.instance.IsPlayerTurn == false)
			{
				RemoveFromGame();
			}
		}
	}
}