using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Spends some people juice to restore a bunch of generic people juice!
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class PlainBroth : ActionCardBase
    {
        // Can't target anything
        public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        public override bool CanTargetPlayer => false;

        public override void OnTarget(Card.CardBase _) {
            for(var i = 0; i < properties["toRestore"]; i++)
                CardGameManager.instance.currentPeopleJuice.Add(PeopleJuice.Types.Generic);

            SendToGraveyard();
        }
    }
}
