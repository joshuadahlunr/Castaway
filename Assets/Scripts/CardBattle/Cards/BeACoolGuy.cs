namespace CardBattle {

    /// <summary>
    /// Card which refills the cast's health
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class BeACoolGuy : Card.ActionCardBase {
        // Can only target monsters and equipment
        public override bool CanTargetPlayer => false;

        /// <summary>
        /// When the card is cast, reset the caster's health to be equal to their max health
        /// </summary>
        public override void OnTarget(Card.CardBase target) {
            if (OwnedByPlayer) {
                var state = CardGameManager.instance.playerHealthState;
                state.health = CardGameManager.instance.playerHealthState.maxHealth;
                CardGameManager.instance.playerHealthState = state;
            } else {
                var state = OwningMonster.healthState;
                state.health = OwningMonster.healthState.maxHealth;
                OwningMonster.healthState = state;
            }
        }
    }
}