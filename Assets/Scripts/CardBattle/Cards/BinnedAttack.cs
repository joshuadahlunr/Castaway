namespace CardBattle {
    /// <summary>
    /// Attack that is removed from the game instead of sent to the graveyard
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class BinnedAttack : Attack {
        /// <summary>
        /// Binned attacks aren't sent to the graveyard... they are removed from the game!
        /// </summary>
        public override void SendToGraveyard() => RemoveFromGame();
    }
}
