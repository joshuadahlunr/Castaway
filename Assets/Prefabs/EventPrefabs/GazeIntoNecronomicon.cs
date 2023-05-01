using CardBattle.Containers;
using CardBattle;
using Crew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

	/// <summary>
    /// Gaze into the Necronomicon Event: Player either kills a crewmate or loses the game.
    /// </summary>
	/// <author>Dana Conley</author>

namespace RandomEvents {

    public class GazeIntoNecronomicon : MonoBehaviour
    {
        public Text eventText;
        public Button loseGameButton;
        public Button killCrewmateButton;

        void Start()
        {
            // buttons
            killCrewmateButton.onClick.AddListener(KillCrewmate);
            loseGameButton.onClick.AddListener(LoseGame);
        }

        void LoseGame()
        {
            // player loses the game
        

            EndEvent();        
        }

        void KillCrewmate()
        {
            // player chooses a crewmate to kill

            CardContainerBase card = new CardContainerBase();
            card.RemoveCard(1);

            EndEvent();
        }

        void EndEvent()
        {
            killCrewmateButton.gameObject.SetActive(false);
            loseGameButton.gameObject.SetActive(false);
        }

    }
}