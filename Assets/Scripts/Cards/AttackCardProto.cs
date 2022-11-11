using System.Linq;

public class AttackCardProto : Card.CardBase {
    // Example modification that multiplies damage values by 10
    public class DamageTimes10Modification : Modification {
        public override PropertyDictionary GetProperties(PropertyDictionary _props) {
            var props = _props.Clone();

            var found = false;
            foreach (var name in props.Keys.ToArray()) {
                var prop = props[name];
                if (prop.type != Property.Type.Damage) continue;
                props[name] = new Property() { type = Property.Type.Damage, value = prop.value * 10 };
                found = true;
            }
            return found ? props : _props;
        }
    }
    
    public new void Start() {
        AddModification(new DamageTimes10Modification());
        base.Start();
    }
}
