using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardBattle.Card;
using Extensions;
using UnityEngine;

namespace CardBattle {
    /// <summary>
    ///     Card which summons a random selkie
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Summoning : ActionCardBase {
	    // This card cannot target any object
		public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
		// This card cannot target players directly
		public override bool CanTargetPlayer => false;

        public override void OnTarget(CardBase _) {
	        // Pick random monsters from the database until we find one that can become a selkie
	        var selkie = CardGameManager.instance.monsterDatabase.cards.Keys.Shuffle().First();
	        while(CardGameManager.instance.monsterDatabase.cards[selkie].monsterCard.selkieCard == null)
		        selkie = CardGameManager.instance.monsterDatabase.cards.Keys.Shuffle().First();

	        // Spawn the selkie
	        var newSelkie = CardGameManager.instance.monsterDatabase.Instantiate(selkie).PromoteToSelkie();
	        newSelkie.Position();
	        CardGameManager.instance.monsters = new List<MonsterCardBase>(CardGameManager.instance.monsters) { newSelkie }.ToArray();

	        // Have it take its selkie form after it has had a frame to settle!
	        IEnumerator BecomeSelkieNextFrame() {
		        yield return null;
		        newSelkie.BecomeSelkie();
	        }
	        newSelkie.StartCoroutine(BecomeSelkieNextFrame());

	        SendToGraveyard();
        }
	}
}