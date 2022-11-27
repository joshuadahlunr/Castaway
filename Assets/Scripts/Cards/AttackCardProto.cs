using System.Linq;

public class AttackCardProto : Card.ActionCardBase {
    // Example modification that multiplies damage values by 10
    public class DamageTimesXModification : Modification {
        public float X = 10;
        
        public override PropertyDictionary GetProperties(PropertyDictionary _props) {
            var props = _props.Clone();

            var found = false;
            foreach (var name in props.Keys.ToArray()) {
                var prop = props[name];
                if (prop.type != Property.Type.Damage) continue;
                props[name] = new Property() { type = Property.Type.Damage, value = (int)(prop.value * X) };
                found = true;
            }
            return found ? props : _props;
        }
    }
    
    public void Awake() {
        AddModification(new DamageTimesXModification());
    }

    public override void OnStateChanged(State oldState, State newState) {
        if (newState == State.InHand)
            if (modifications[0] is DamageTimesXModification mod) {
                mod.X = container.Index(this) + 1;
            }
    }

    // When the player targets something
    public override void OnTarget(Card.CardBase _target) {
        var target = _target.GetComponent<Card.HealthCardBase>();
        
        // If the target is null, and we are the player then send us back to their hand!
        // If the target is null, and we are a monster then DamageTargetOrPlayer handles it
        if (target is null && OwnedByPlayer) { 
            GetComponent<Targeting>().Reset();
            return;
        }
        
        // Damage target (falling back to player if we are monster and not targeting anything!)
        DamageTargetOrPlayer(properties["primary"], target);
        
        SendToGraveyard();
    }
}
