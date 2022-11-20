public class MonsterDeck : Deck {
	public void OnTurnStart() {
		if (CardGameManager.instance.IsPlayerTurn) return; // Don't do anything on the player's turn
		
		Shuffle();
		// TODO: Implement damage
	}
}
