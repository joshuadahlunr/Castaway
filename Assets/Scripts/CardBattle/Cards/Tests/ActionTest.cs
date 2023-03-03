using UnityEngine;

namespace CardBattle.TestCards {

    /// <summary>
    /// Basic attack card
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class ActionTest : Card.ActionCardBase {
        // Can target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.None;
        public override CardFilterer.CardFilters MonsterTargetingFilters => CardFilterer.CardFilters.None;
        
        public override void OnTarget(Card.CardBase target) {
            Debug.Log($"targeting: {target.name}");
            SendToGraveyard();
        }
        
        public override void OnTargeted(Card.CardBase targeter) {
            Debug.Log($"targeted by: {targeter.name}");
        }
    }
}