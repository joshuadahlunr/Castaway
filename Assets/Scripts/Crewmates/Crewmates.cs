using System;
using UnityEngine;
using UnityEngine.UI;
using CardBattle.Card;
using TMPro;

namespace Crew {
    /// <summary>
    ///     Class for crewmates storing associated information such as name, status, crew card, etc.
    /// </summary>
    /// <author> Misha Desear </author>

    public class Crewmates : MonoBehaviour
    {
        // Numeric code generated upon spawn for keeping track of card
        [SerializeField] private int _code;
        public int Code
        {
            get => _code;
            set { _code = value; }
        }

        // Enum denoting status of crewmate with regards to whether or not they are part of the crew
        [Serializable]
        public struct Status {
            public enum CrewTag {
                NotInCrew,  // 0
                WasInCrew,  // 1
                InCrew      // 2
            }
            public int value;
            public static implicit operator int(Status s) => s.value;
        }

        [SerializeField] private Status.CrewTag _crewTag;
        public Status.CrewTag CrewTag
        {
            get => _crewTag;
            set { _crewTag = value; }
        }

        // Enum denoting type of crewmate to ensure appropriate associated card/uniform
        [Serializable]
        public struct CrewClass {
            public enum Type {
                Invalid,
                Wizard,         // 1
                Navigator,      // 2
                Entertainer,    // 3
                Engineer,       // 4
                Cook,           // 5
                Occultist,      // 6
                Mercenary,      // 7
                Deckhand        // 8

            }
            public int value;
        }

        [SerializeField] private CrewClass.Type _type;
        public CrewClass.Type Type {
            get => _type;
            set { _type = value; }
        }

        // The crewmate's name
        [SerializeField] private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        // The crewmate's current level
        [SerializeField] private int _level;
        public int Level
        {
            get => _level;
            set { _level = value; }
        }

        // The crewmate's current morale
        [SerializeField] private int _morale;
        public int Morale
        {
            get => _morale;
            set { _morale = value; }
        }

        // The crewmate's current amount of XP
        [SerializeField] private int _currentXp;
        public int CurrentXP
        {
            get => _currentXp;
            set { _currentXp = value; }
        }

        // The amount of XP needed for the crewmate to reach the next level
        [SerializeField] private int _xpNeeded;
        public int XPNeeded
        {
            get => _xpNeeded;
            set { _xpNeeded = value; }
        }

        // The texture of the crewmate's base
        [SerializeField] private Texture2D _baseSprite;
        public Texture2D BaseSprite
        {
            get => _baseSprite;
            set { _baseSprite = value; }
        }

        // The texture of the crewmate's hair
        [SerializeField] private Texture2D _hairSprite;
        public Texture2D HairSprite
        {
            get => _hairSprite;
            set { _hairSprite = value; }
        }

        // The texture of the crewmate's eyes
        [SerializeField] private Texture2D _eyeSprite;
        public Texture2D EyeSprite
        {
            get => _eyeSprite;
            set { _eyeSprite = value; }
        }

        // The texture of the crewmate's eyebrows
        [SerializeField] private Texture2D _browSprite;
        public Texture2D BrowSprite
        {
            get => _browSprite;
            set { _browSprite = value; }
        }

        // The texture of the crewmate's mouth
        [SerializeField] private Texture2D _mouthSprite;
        public Texture2D MouthSprite
        {
            get => _mouthSprite;
            set { _mouthSprite = value; }
        }

        // The texture of the crewmate's clothes
        [SerializeField] private Texture2D _clothesSprite;
        public Texture2D ClothesSprite
        {
            get => _clothesSprite;
            set { _clothesSprite = value; }
        }

        // The crewmate's associated card to be added to the deck
        [SerializeField] private string _crewCard;
        public string CrewCard
        {
            get => _crewCard;
            set { _crewCard = value; }
        }

        /// <summary>
        ///     Shows the crewmate's information (e.g., name, type, level, current XP and morale) 
        ///     using the information panel prefab in the scene
        /// </summary>
        public void ShowInfo()
        {
            GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(2, 2, 1);

            GameObject displayName = GameObject.FindGameObjectWithTag("Crew Name");
            displayName.GetComponent<TextMeshProUGUI>().text = _name.ToString();

            GameObject typeName = GameObject.FindGameObjectWithTag("Crew Type");
            typeName.GetComponent<TextMeshProUGUI>().text = "the " + _type.ToString();

            GameObject morale = GameObject.FindGameObjectWithTag("Morale");
            morale.GetComponent<TextMeshProUGUI>().text = "Morale: " + _morale.ToString();
            GameObject moraleSlider = GameObject.FindGameObjectWithTag("Morale Slider");
            moraleSlider.GetComponent<Slider>().value = _morale;

            GameObject level = GameObject.FindGameObjectWithTag("Level");
            level.GetComponent<TextMeshProUGUI>().text = "Level " + _level.ToString() + ": " + _currentXp.ToString()
                                                        + "/" + _xpNeeded.ToString() + " XP";

            GameObject xpSlider = GameObject.FindGameObjectWithTag("XP Slider");
            xpSlider.GetComponent<Slider>().value = _currentXp;
            xpSlider.GetComponent<Slider>().maxValue = _xpNeeded;
        }

        /// <summary>
        ///     Hides the information panel prefab in the scene
        /// </summary>
        public void HideInfo()
        {
            GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(0, 0, 0);
        }
    }
}