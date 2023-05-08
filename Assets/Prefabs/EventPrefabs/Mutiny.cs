using CardBattle.Containers;
using CardBattle;
using Crew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

	/// <summary>
    /// Mutiny Random Event: Player either loses a card or lowers crew morale. Ship health gets -1
    /// </summary>
	/// <author>Dana Conley</author>

namespace RandomEvents {

    public class Mutiny : MonoBehaviour
    {
        public Text eventText;
        public Button loseCardsButton;
        public Button lowerMoraleButton;
        public 

        //public override CardFilterer.CardFilters TargetingFilters => CardFilterer.CardFilters.All;
        //public override bool CanTargetPlayer => false;

        void Start()
        {
            // buttons
            lowerMoraleButton.onClick.AddListener(LowerMorale);
            loseCardsButton.onClick.AddListener(LoseCards);
        }

        void LoseCards()
        {
            // ship loses 1 hp

            //CardGameManager ship = new CardGameManager();
            //ship.shipLevel = shipLevel -1;

            // player loses a card

            CardContainerBase card = new CardContainerBase();
            card.RemoveCard(1);
            //target.RemoveFromGame();
            EndEvent();
        }

        void LowerMorale()
        {
            // ship loses 1 hp

            //CardGameManager ship = new CardGameManager();
            //ship.shipLevel = shipLevel -1;

            // crew morale is lowered
            Crewmates crewmate = new Crewmates();
            //crewmate.DecreaseMorale();

            EndEvent();
        }

        void EndEvent()
        {
            loseCardsButton.gameObject.SetActive(false);
            lowerMoraleButton.gameObject.SetActive(false);
        }
    }
}