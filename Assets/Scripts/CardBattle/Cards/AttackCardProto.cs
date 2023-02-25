using System.Linq;

namespace CardBattle {

    /// <summary>
    /// Basic prototype attack card used for testing
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class AttackCardProto : Card.ActionCardBase {
        // Can only target monsters and equipment
        public override CardFilterer.CardFilters TargetingFilters =>
            ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay/*| CardFilterer.CardFilters.Equipment*/);
        public override CardFilterer.CardFilters MonsterTargetingFilters =>
            TargetingFilters | CardFilterer.CardFilters.Monster;

        /// <summary>
        /// Example modification that multiplies damage values by some factor
        /// </summary>
        public class DamageTimesXModification : Modification {
            /// <summary>
            /// Amount to multiply damage by
            /// </summary>
            public float X = 1;

            /// <summary>
            /// When we try to get the properties, multiply any property tagged as damage by <see cref="X"/>
            /// </summary>
            public override PropertyDictionary GetProperties(PropertyDictionary _props) {
                // Copy of the properties dictionary, set to null until we find a property that needs to be modified
                PropertyDictionary props = null;

                // Scan through every property and note if we found any damage properties
                foreach (var name in _props.Keys.ToArray()) {
                    var prop = _props[name];

                    // Skip any non-damage properties
                    if (prop.tag != Property.Tag.Damage) continue;
                    props ??= _props.Clone(); // Only clone the dictionary the first time we encounter a damage property

                    // Save the property with its value multiplied by X back to the copied dictionary
                    props[name] = new Property() { tag = Property.Tag.Damage, value = (int)(prop.value * X) };
                }

                // Return the modified dictionary if we changed anything or the original otherwise...
                return props ?? _props;
            }
        }

        /// <summary>
        /// When the game starts add a <see cref="DamageTimesXModification"/> to the card 
        /// </summary>
        public new void Awake() {
            base.Awake();
            AddModification(new DamageTimesXModification());
        }

        /// <summary>
        /// When the card is added to the hand, change its damage multiplier to reflect its position in the hand
        /// </summary>
        public override void OnStateChanged(State oldState, State newState) {
            if (newState == State.InHand)
                if (modifications[0] is DamageTimesXModification mod) {
                    mod.X = container.Index(this) + 1;
                }
        }

        /// <summary>
        /// When the player targets something, damage it (if it exists) and then send this card to the graveyard
        /// </summary>
        public override void OnTarget(Card.CardBase _target) {
            var target = _target?.GetComponent<Card.HealthCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            // Damage target (falling back to player if we are monster and not targeting anything!)
            DamageTargetOrPlayer(properties["primary"], target);

            SendToGraveyard();
        }
    }
}