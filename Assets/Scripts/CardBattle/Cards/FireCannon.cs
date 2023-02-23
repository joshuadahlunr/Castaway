using CardBattle.Card;

namespace CardBattle {

    public class FireCannon : Card.ActionCardBase {
        // Can't target anything but in play equipment...
        public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);
        
        public override void OnTarget(Card.CardBase _target) {
            if (_target is not EquipmentCardBase target) {
                RefundAndReset();
                return;
            }
            
            foreach (var monster in target.GetCollidingCards<MonsterCardBase>()) 
                monster.healthState = monster.healthState.ApplyDamage(properties["primary"]);

            SendToGraveyard();
        }
    }
}
