using CardBattle.Card;
using CardBattle.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CardBattle {
	/// <summary>
	///     Class for the basic Attack card
	/// </summary>
	/// <author>Jared Whitel</author>
	public class FireBomb : ActionCardBase {
		// Can only target monsters and equipment
		public override CardFilterer.CardFilters TargetingFilters
			=> ~(CardFilterer.CardFilters.Monster |
			     CardFilterer.CardFilters.InPlay | CardFilterer.CardFilters.Equipment);

		// Can target monsters in addition to everything allowed by TargetingFilters
		public override CardFilterer.CardFilters MonsterTargetingFilters
			=> TargetingFilters | CardFilterer.CardFilters.Monster;

        // Variable to add the status effect card
        [SerializeField] private Card.StatusCardBase burn;

		/// <summary>
		///     Called when the player targets something with this card
		/// </summary>
		/// <param name="_target">The target card</param>
		public override void OnTarget(CardBase _target) {
            var target = _target?.GetComponent<Card.MonsterCardBase>();
            if (NullAndPlayerCheck(target)) return; // Make sure the target isn't null if owned by the player

            
			// Damage the target
			DamageTargetOrPlayer(1, target);
            var inst = Instantiate(burn);
            target.deck.AddCard(inst);
            if(OwnedByPlayer) {
                inst.cardOwner = target.cardOwner;
            } 

            CardGameManager.instance.CheckWinLose();
			// Send this card to the graveyard
			SendToGraveyard();
		}
	}
}