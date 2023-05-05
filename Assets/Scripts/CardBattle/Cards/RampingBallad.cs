using CardBattle.Card;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SQLite;

namespace CardBattle
{
    /// <summary>
    ///     Ramping Ballad: keeps count of how many times this card has been played
    ///     in the current run and adds damage equivalent to number of times played
    /// </summary>
    /// <author> Misha Desear </author>
    public class RampingBallad : Card.ActionCardBase
    {
        /// <summary>
        ///     Stores number of times this card has been played
        ///     and the crewmate this card is associated with
        /// </summary>
        public class BalladData
        {
            /// <summary>
            ///     The unique identifier of this table
            /// </summary>
            [PrimaryKey, Unique, AutoIncrement]
            public int id { set; get; }

            /// <summary>
            ///     The amount of times that this card
            ///     has been played in this run
            /// </summary>
            public int balladCount { set; get; }

            /// <summary>
            ///     The crewmate associated with this card
            /// </summary>
            public int? associatedCrewmateID { set; get; }
        }

        // Can only target monsters
        public override CardFilterer.CardFilters TargetingFilters
            => ~(CardFilterer.CardFilters.Monster | CardFilterer.CardFilters.InPlay);

        // Holds the count and ID retrieved from SQL
        private BalladData balladCounter;

        public override void OnTarget(CardBase _target)
        {
            // Obtain health of target
            var target = _target?.GetComponent<HealthCardBase>();

            // Obtain the CardBase component of this instance
            var cardBase = this.GetComponent<CardBase>();

            // Retrieve BalladData from SQL associated with the owner of this card
            var counter = DatabaseManager.GetOrCreateTable<BalladData>()
                .FirstOrDefault(b => b.associatedCrewmateID == cardBase.associatedCrewmate);

            // If null, set default values
            if (counter is null)
            {
                balladCounter.balladCount = 0;
                balladCounter.associatedCrewmateID = cardBase.associatedCrewmate;
            }

            // Else, set values equal to SQL output
            else
            {
                balladCounter.balladCount = counter.balladCount;
            }

            // Return to hand if target is null or belongs to the player
            if (NullAndPlayerCheck(target)) return;

            // Deal damage plus additional damage for number of times used
            DamageTargetOrPlayer(properties["primary"] + balladCounter.balladCount, target);

            // Increment the counter by 1
            balladCounter.balladCount += 1;

            // Retrieve the table again and delete it (if it exists)
            var table = DatabaseManager.GetOrCreateTable<BalladData>()
                .FirstOrDefault(b => b.associatedCrewmateID == cardBase.associatedCrewmate);
            if (table != null)
            {
                DatabaseManager.database.Delete(table);
            }

            // Insert an updated version of the BalladData
            DatabaseManager.database.Insert(new BalladData {
                balladCount = balladCounter.balladCount,
                associatedCrewmateID = cardBase.associatedCrewmate
            });

            // Send to graveyard
            SendToGraveyard();
        }
    }
}