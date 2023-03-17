using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using CardBattle.Card;
using Crew.Globals;
using TMPro;
using Random = UnityEngine.Random;

namespace Crew {
    /// <summary>
    /// <author>Misha Desear</author>
    /// </summary>
    public class Crewmates : MonoBehaviour
    {
        /// <summary>
        /// Holds the prefab for the information panel to display stats
        /// </summary>
        [SerializeField] private GameObject _infoPanel;

        public Crew.Globals.Global global;
        public GameObject infoPanel => _infoPanel; 

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

        [Serializable]
        public struct Class {
            public enum Type {
                Wizard,         // 0
                Navigator,      // 1
                Entertainer,    // 2
                Engineer,       // 3
                Cook,           // 4
                Occultist,      // 5
                Mercenary,      // 6
                Deckhand        // 7

            }
            public int value;
            public static T RandomEnumValue<T>()
            {
                var v = Enum.GetValues(typeof(T));
                return (T) v.GetValue(Random.Range(0, 7));
            }
        }

        // TODO: implement preferences as enums rather than class objects

        [SerializeField] private Class.Type _type;
        public Class.Type type {
            get => _type;
            set { _type = value; }
        }

        [SerializeField] private string _name;
        public string Name 
        {
            get => _name;
            set { _name = value; }
        }

        [SerializeField] private int _level;
        public int Level 
        {
            get => _level;
            set { _level = value; }
        }

        [SerializeField] private int _morale;
        public int Morale
        {
            get => _morale;
            set { _level = value; }
        }

        [SerializeField] private int _currentXp;
        public int CurrentXP 
        {
            get => _currentXp;
            set { _currentXp = value; }
        }

        [SerializeField] private int _xpNeeded;
        public int XPNeeded
        {
            get => XPNeeded;
            set { _xpNeeded = value; }
        }

        [SerializeField] private Sprite _crewSprite;
        public Sprite crewSprite
        {
            get => _crewSprite;
            set { _crewSprite = value; }
        }
        
        [SerializeField] private ActionCardBase _crewCard;
        public ActionCardBase crewCard 
        {
            get => _crewCard;
            set { _crewCard = value; }
        } 

        public void ShowInfo()
        {
            GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(1, 1, 1);

            GameObject displayName = GameObject.FindGameObjectWithTag("Crew Name");
            displayName.GetComponent<TextMeshProUGUI>().text = "the " + _type.ToString();

            GameObject morale = GameObject.FindGameObjectWithTag("Morale");
            morale.GetComponent<TextMeshProUGUI>().text = "Morale: " + _morale.ToString();
            GameObject moraleSlider = GameObject.FindGameObjectWithTag("Morale Slider");
            moraleSlider.GetComponent<Slider>().value = _morale;

            GameObject level = GameObject.FindGameObjectWithTag("Level");
            level.GetComponent<TextMeshProUGUI>().text = "Level " + _level.ToString() + " (" + _currentXp.ToString()
                                                        + "/" + _xpNeeded.ToString() + ")";

            GameObject xpSlider = GameObject.FindGameObjectWithTag("XP Slider");
            xpSlider.GetComponent<Slider>().value = _currentXp;
            xpSlider.GetComponent<Slider>().maxValue = _xpNeeded;
        }

        public void HideInfo() 
        {
            GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(0, 0, 0);
        }

        public void GenerateNewCrewmate()
        {
            Crewmates newCrew = new();
            newCrew._crewTag = 0;
            newCrew._level = 1; 
            newCrew._xpNeeded = 10;
            newCrew._currentXp = 0;
            newCrew._morale = 50;
            newCrew._type = Class.RandomEnumValue<Class.Type>();
            newCrew._crewSprite = null;
            CardDatabase.Instantiate(newCrew);
        }
    }
}