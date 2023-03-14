using System;

namespace CardBattle {
	/// <summary>
    ///     Struct representing the health and damage reduction of an entity in a game.
    /// </summary>
    /// <author>Joshua Dahl</author>
    [Serializable]
	public struct HealthState {
        /// <summary>
        ///     Current health value.
        /// </summary>
        public int health;

        /// <summary>
        ///     Maximum health value.
        /// </summary>
        public int maxHealth;

        /// <summary>
        ///     Permanent damage reduction value.
        /// </summary>
        public int permanentDamageReduction;

        /// <summary>
        ///     Temporary damage reduction value.
        /// </summary>
        public int temporaryDamageReduction;

        /// <summary>
        ///     Total damage reduction value.
        /// </summary>
        public int TotalDamageReduction => permanentDamageReduction + temporaryDamageReduction;

        /// <summary>
        /// Implicitly converts a HealthState object to an integer, returning its health value.
        /// </summary>
        /// <param name="h">The HealthState object to convert.</param>
        /// <returns>The health value of the HealthState object.</returns>
        public static implicit operator int(HealthState h) => h.health;

        /// <summary>
        ///     Applies the given damage to the entity's health value, after subtracting the total damage reduction value.
        /// </summary>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState ApplyDamage(int damage) {
	        var temp = Math.Max(damage, 0); // set temp to the maximum of damage and 0

			temp = Math.Max(damage - temporaryDamageReduction, 0); // subtract temporary damage reduction from damage and set temp to the maximum of the result and 0
			temporaryDamageReduction = Math.Max(temporaryDamageReduction - damage, 0); // subtract damage from temporary damage reduction and set temporaryDamageReduction to the maximum of the result and 0
			damage = temp; // set damage to temp

			temp = Math.Max(damage - permanentDamageReduction, 0); // subtract permanent damage reduction from damage and set temp to the maximum of the result and 0
			permanentDamageReduction = Math.Max(permanentDamageReduction - damage, 0); // subtract damage from permanent damage reduction and set permanentDamageReduction to the maximum of the result and 0
			damage = temp; // set damage to temp

			health -= damage; // subtract the final damage value from health
			return this;
		}

        /// <summary>
        ///     Applies the given healing to the entity's health value, if it's positive; or applies the equivalent damage if it's negative.
        /// </summary>
        /// <param name="healing">The amount of healing to apply.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState ApplyHealing(int healing) {
			if (healing < 0)
				return ApplyDamage(-healing);
			health += healing;
			return this;
		}

        /// <summary>
        ///     Sets the permanent damage reduction value to the given value.
        /// </summary>
        /// <param name="value">The new permanent damage reduction value.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState SetPermanentDamageReduction(int value) {
			permanentDamageReduction = value;
			return this;
		}

        /// <summary>
        ///     Adds the given value to the permanent damage reduction value.
        /// </summary>
        /// <param name="value">The amount to add to the permanent damage reduction value.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState AddPermanentDamageReduction(int value) {
			permanentDamageReduction += value;
			return this;
		}

        /// <summary>
        ///     Sets the temporary damage reduction value to the given value.
        /// </summary>
        /// <param name="value">The new temporary damage reduction value.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState SetTemporaryDamageReduction(int value) {
			temporaryDamageReduction = value;
			return this;
		}

        /// <summary>
        ///     Adds the given value to the temporary damage reduction value.
        /// </summary>
        /// <param name="value">The amount to add to the temporary damage reduction value.</param>
        /// <returns>The updated HealthState.</returns>
        public HealthState AddTemporaryDamageReduction(int value) {
	        temporaryDamageReduction += value;
			return this;
		}
	}
}