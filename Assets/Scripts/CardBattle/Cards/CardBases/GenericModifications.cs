using System;
using System.Linq;

namespace CardBattle.Card.Modifications.Generic {
	/// <summary>
	///     A modification that multiplies certain card properties based on a specified level.
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class LevelModification : MultiplicationModification {
		public LevelModification(int level, float multiplierPerLevel = .25f) {
			DamageMultiplier = 1 + (level - 1) * multiplierPerLevel;
			HealthPropertiesMultiplier = 1 + (level - 1) * multiplierPerLevel;
			HealthMultiplier = 1 + (level - 1) * multiplierPerLevel;
			BlockMultiplier = 1 + (level - 1) * multiplierPerLevel;
			DrawMultiplier = 1 + (level - 1) * multiplierPerLevel;
			UtilityMultiplier = 1 + (level - 1) * multiplierPerLevel;
		}
	}

	/// <summary>
	///     A modification that reduces certain card properties by a specified amount.
	/// </summary>
	public class ReductionModification : HealthCardBase.Modification {
		public int DamageReductionAmount = 0;
		public int HealthPropertiesReductionAmount = 0;
		public int HealthReductionAmount = 0;
		public int BlockReductionAmount = 0;
		public int DrawReductionAmount = 0;
		public int UtilityReductionAmount = 0;

		/// <summary>
		///     Modifies the given property dictionary by reducing certain card properties.
		/// </summary>
		public override CardBase.PropertyDictionary GetProperties(CardBase.PropertyDictionary _props) {
			// Copy of the properties dictionary, set to null until we find a property that needs to be modified
			var props = _props.Clone();

			// Scan through every property and note if we found any damage properties
			foreach (var name in _props.Keys.ToArray()) {
				var prop = _props[name];

				props[name] = prop.tag switch {
					CardBase.Property.Tag.Damage => new CardBase.Property {
						tag = CardBase.Property.Tag.Damage, value = prop.value - DamageReductionAmount
					},
					CardBase.Property.Tag.Health => new CardBase.Property {
						tag = CardBase.Property.Tag.Health, value = prop.value - HealthPropertiesReductionAmount
					},
					CardBase.Property.Tag.Block => new CardBase.Property {
						tag = CardBase.Property.Tag.Block, value = prop.value - BlockReductionAmount
					},
					CardBase.Property.Tag.Draw => new CardBase.Property {
						tag = CardBase.Property.Tag.Draw, value = prop.value - DrawReductionAmount
					},
					CardBase.Property.Tag.Utility => new CardBase.Property {
						tag = CardBase.Property.Tag.Utility, value = prop.value - UtilityReductionAmount
					},
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			// Return the modified dictionary if we changed anything or the original otherwise...
			return props ?? _props;
		}

		/// <summary>
		///     Modifies the given health state by reducing the health property.
		/// </summary>
		public override HealthState GetHealth(HealthState health) {
			health.health -= HealthReductionAmount;
			return health;
		}
	}

	/// <summary>
	///     A modification that reduces the cost of an action card by a certain amount.
	/// </summary>
	public class CostReductionModification : ActionCardBase.Modification {
		/// <summary>
		///     The amount by which to reduce the cost.
		/// </summary>
		public PeopleJuice.Cost deduction;

		/// <inheritdoc />
		public override PeopleJuice.Cost GetCost(PeopleJuice.Cost _cost) {
			var cost = new PeopleJuice.Cost(_cost);
			PeopleJuice.DeductCost(ref cost, deduction);
			return cost;
		}
	}

	/// <summary>
	///     A modification that increases the cost of an action card by a certain amount.
	/// </summary>
	public class CostIncreaseModification : ActionCardBase.Modification {
		/// <summary>
		///     The amount by which to increase the cost.
		/// </summary>
		public PeopleJuice.Cost increase;

		/// <inheritdoc />
		public override PeopleJuice.Cost GetCost(PeopleJuice.Cost _cost) {
			var cost = new PeopleJuice.Cost(_cost);
			cost.AddRange(increase);
			return cost;
		}
	}

	public class MultiplicationModification : HealthCardBase.Modification {
		/// <summary>
		///     The multiplier to apply to damage properties.
		/// </summary>
		public float DamageMultiplier = 1;

		/// <summary>
		///     The multiplier to apply to health-related properties.
		/// </summary>
		public float HealthPropertiesMultiplier = 1;

		/// <summary>
		///     The multiplier to apply to a health state's max health value.
		/// </summary>
		public float HealthMultiplier = 1;

		/// <summary>
		///     The multiplier to apply to block properties.
		/// </summary>
		public float BlockMultiplier = 1;

		/// <summary>
		///     The multiplier to apply to draw properties.
		/// </summary>
		public float DrawMultiplier = 1;

		/// <summary>
		///     The multiplier to apply to utility properties.
		/// </summary>
		public float UtilityMultiplier = 1;

		/// <inheritdoc />
		public override CardBase.PropertyDictionary GetProperties(CardBase.PropertyDictionary _props) {
			// Copy of the properties dictionary, set to null until we find a property that needs to be modified
			var props = _props.Clone();

			// Scan through every property and note if we found any damage properties
			foreach (var name in _props.Keys.ToArray()) {
				var prop = _props[name];

				props[name] = prop.tag switch {
					CardBase.Property.Tag.Damage => new CardBase.Property {
						tag = CardBase.Property.Tag.Damage, value = (int)(prop.value * DamageMultiplier)
					},
					CardBase.Property.Tag.Health => new CardBase.Property {
						tag = CardBase.Property.Tag.Health, value = (int)(prop.value * HealthPropertiesMultiplier)
					},
					CardBase.Property.Tag.Block => new CardBase.Property {
						tag = CardBase.Property.Tag.Block, value = (int)(prop.value * BlockMultiplier)
					},
					CardBase.Property.Tag.Draw => new CardBase.Property {
						tag = CardBase.Property.Tag.Draw, value = (int)(prop.value * DrawMultiplier)
					},
					CardBase.Property.Tag.Utility => new CardBase.Property {
						tag = CardBase.Property.Tag.Utility, value = (int)(prop.value * UtilityMultiplier)
					},
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			// Return the modified dictionary if we changed anything or the original otherwise...
			return props ?? _props;
		}

		/// <inheritdoc />
		public override HealthState GetHealth(HealthState health) =>
			// health.maxHealth = (int)(health.maxHealth * HealthMultiplier);
			// health.health = (int)(health.health * HealthMultiplier);
			// TODO: Not working!
			health;
	}

	/// <summary>
	///     A card modification that removes the cost of the card.
	/// </summary>
	internal class RemoveCostModification : ActionCardBase.Modification {
		/// <inheritdoc />
		public override PeopleJuice.Cost GetCost(PeopleJuice.Cost _) => new();
	}
}