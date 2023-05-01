using CardBattle.Card;
using Crew;

namespace CardBattle {
	/// <summary>
	///     Class for a card which kills a crewmate
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class SandalThrow : KillCrewmate {

		// When sandle throw would be discarded it is instead removed from the game!
		public override void SendToGraveyard() => RemoveFromGame();
	}
}