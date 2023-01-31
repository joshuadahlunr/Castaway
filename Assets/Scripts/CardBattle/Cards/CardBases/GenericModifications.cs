using System;
using System.Linq;

namespace CardBattle.Card.Modifications.Generic {
	public class LevelModification : MultiplicationModification {
		public LevelModification(int level, float multiplierPerLevel = .5f) {
			DamageMultiplier = 1 + (level - 1) * multiplierPerLevel;
			HealthPropertiesMultiplier = 1 + (level - 1) * multiplierPerLevel;
			HealthMultiplier = 1 + (level - 1) * multiplierPerLevel;
			BlockMultiplier = 1 + (level - 1) * multiplierPerLevel;
			DrawMultiplier = 1 + (level - 1) * multiplierPerLevel;
			UtilityMultiplier = 1 + (level - 1) * multiplierPerLevel;
		}
	}
	
	public class ReductionModification : HealthCardBase.Modification {
		public int DamageReductionAmount = 0;
		public int HealthPropertiesReductionAmount = 0;
		public int HealthReductionAmount = 0;
		public int BlockReductionAmount = 0;
		public int DrawReductionAmount = 0;
		public int UtilityReductionAmount = 0;

		public override CardBase.PropertyDictionary GetProperties(CardBase.PropertyDictionary _props) {
			// Copy of the properties dictionary, set to null until we find a property that needs to be modified
			CardBase.PropertyDictionary props = _props.Clone();

			// Scan through every property and note if we found any damage properties
			foreach (var name in _props.Keys.ToArray()) {
				var prop = _props[name];

				props[name] = prop.tag switch {
					CardBase.Property.Tag.Damage => new CardBase.Property() {
						tag = CardBase.Property.Tag.Damage, value = (prop.value - DamageReductionAmount)
					},
					CardBase.Property.Tag.Health => new CardBase.Property() {
						tag = CardBase.Property.Tag.Health, value = (prop.value - HealthPropertiesReductionAmount)
					},
					CardBase.Property.Tag.Block => new CardBase.Property() {
						tag = CardBase.Property.Tag.Block, value = (prop.value - BlockReductionAmount)
					},
					CardBase.Property.Tag.Draw => new CardBase.Property() {
						tag = CardBase.Property.Tag.Draw, value = (prop.value - DrawReductionAmount)
					},
					CardBase.Property.Tag.Utility => new CardBase.Property() {
						tag = CardBase.Property.Tag.Utility, value = (prop.value - UtilityReductionAmount)
					},
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			// Return the modified dictionary if we changed anything or the original otherwise...
			return props ?? _props;
		}

		public override HealthState GetHealth(HealthState health) {
			health.health -= HealthReductionAmount;
			return health;
		}
	}

	public class CostReductionModification : ActionCardBase.Modification {
		public PeopleJuice.Cost deduction;
		
		public override PeopleJuice.Cost GetCost(PeopleJuice.Cost _cost) {
			var cost = new PeopleJuice.Cost(_cost);
			PeopleJuice.DeductCost(ref cost, deduction);
			return cost;
		}
	}
	
	public class CostIncreaseModification : ActionCardBase.Modification {
		public PeopleJuice.Cost increase;
		
		public override PeopleJuice.Cost GetCost(PeopleJuice.Cost _cost) {
			var cost = new PeopleJuice.Cost(_cost);
			cost.AddRange(increase);
			return cost;
		}
	}

	public class MultiplicationModification : HealthCardBase.Modification {
		public float DamageMultiplier = 1;
		public float HealthPropertiesMultiplier = 1;
		public float HealthMultiplier = 1;
		public float BlockMultiplier = 1;
		public float DrawMultiplier = 1;
		public float UtilityMultiplier = 1;
		
		public override CardBase.PropertyDictionary GetProperties(CardBase.PropertyDictionary _props) {
			// Copy of the properties dictionary, set to null until we find a property that needs to be modified
			CardBase.PropertyDictionary props = _props.Clone();

			// Scan through every property and note if we found any damage properties
			foreach (var name in _props.Keys.ToArray()) {
				var prop = _props[name];

				props[name] = prop.tag switch {
					CardBase.Property.Tag.Damage => new CardBase.Property() {
						tag = CardBase.Property.Tag.Damage, value = (int)(prop.value * DamageMultiplier)
					},
					CardBase.Property.Tag.Health => new CardBase.Property() {
						tag = CardBase.Property.Tag.Health, value = (int)(prop.value * HealthPropertiesMultiplier)
					},
					CardBase.Property.Tag.Block => new CardBase.Property() {
						tag = CardBase.Property.Tag.Block, value = (int)(prop.value * BlockMultiplier)
					},
					CardBase.Property.Tag.Draw => new CardBase.Property() {
						tag = CardBase.Property.Tag.Draw, value = (int)(prop.value * DrawMultiplier)
					},
					CardBase.Property.Tag.Utility => new CardBase.Property() {
						tag = CardBase.Property.Tag.Utility, value = (int)(prop.value * UtilityMultiplier)
					},
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			// Return the modified dictionary if we changed anything or the original otherwise...
			return props ?? _props;
		}

		public override HealthState GetHealth(HealthState health) {
			health.health = (int)(health.health * HealthMultiplier);
			return health;
		}
	}
}