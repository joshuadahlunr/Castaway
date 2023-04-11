using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattle
{
    /// <summary>
    /// Card used to skip turns
    /// <author>Jared White</author>
    /// </summary>
    public class UnplayableCard : Card.ActionCardBase {
        public override void OnTarget(Card.CardBase _) {
            // do nothing!
        }
    }
}
