using System;

namespace CardBattle {

    public class FireCannon : Card.ActionCardBase {
        // Can't target anything but in play equipment...
        public override CardFilterer.CardFilters TargetingFilters => ~(CardFilterer.CardFilters.Equipment | CardFilterer.CardFilters.InPlay);
        

        public override void OnTarget(Card.CardBase _target) {
            throw new NotImplementedException("TODO: Implement fire cannon");

            SendToGraveyard();
        }
    }
}
