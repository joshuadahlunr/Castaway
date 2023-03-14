namespace CardBattle {

    /// <summary>
    /// Card which refills the caster's health
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class BeACoolGuy : Card.ActionCardBase {

        /// <summary>
        /// Determines whether the card can target the player.
        /// This card can only target monsters and equipment, not the player.
        /// </summary>
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// When the card is cast, reset the caster's health to be equal to their max health.
        /// If the card is owned by the player, the player's health is reset.
        /// If the card is owned by a monster, the monster's health is reset.
        /// </summary>
        /// <param name="target">The target of the card (not used in this method).</param>
        public override void OnTarget(Card.CardBase target) {
            // If the card is owned by the player
            if (OwnedByPlayer) {
                // Reset the player's health to their maximum health
                var state = CardGameManager.instance.playerHealthState;
                state.health = CardGameManager.instance.playerHealthState.maxHealth;
                CardGameManager.instance.playerHealthState = state;
            }
            // Otherwise, the card is owned by a monster
            else {
                // Reset the monster's health to their maximum health
                var state = OwningMonster.healthState;
                state.health = OwningMonster.healthState.maxHealth;
                OwningMonster.healthState = state;
            }
        }
    }
}
