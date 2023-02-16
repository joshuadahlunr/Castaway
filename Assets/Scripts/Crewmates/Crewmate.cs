using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;
using UnityEditor;
using Unity.VisualScripting;

/// <summary>
/// @author: Misha Desear
/// </summary>
/// 
public class Crewmate : MonoBehaviour
{
    public GameObject infoPrefab;
    [SerializeField]
    private int _crewLevel, _xpNeeded, _currentXp, _morale;
    [SerializeField]
    private string _crewName, _crewType;
    [SerializeField]
    private bool _inCrew;
    [SerializeField]
    private Preference[] _preferences;

    //TODO: implement card type, likes, and dislikes (as class objects)

    private int defaultMorale = 50; // Between a range of 0-100?
    private string[] randomNames = { "Jeff", "Joe", "Tommy", "Timmy", "David", "Josh", "Misha", "Jared", "Dana"};
    private string[] randomTypes = { "Wizard", "Navigator", "Entertainer", "Engineer", 
                                    "Cook", "Occultist", "Mercenary", "Deckhand" };

    public Crewmate()
    {
    }

    public Preference[] RandomPreferences()
    {
        Preference[] prefList = new Preference[4]; // Limit on # of preferences subject to change
        for (int i = 0; i < prefList.Length; i++)
        {
            Preference pref = new Preference();
            prefList[i] = pref;
        }
        return prefList;
    }

    public string RandomTypes()
    {
        string newType = randomTypes[Random.Range(0, randomTypes.Length)];
        return newType;
    }

    public string RandomName()
    {
        string newName = randomNames[Random.Range(0, randomNames.Length)];
        return newName;
    }

    public void AddXP(int value)
    {
        _xpNeeded += value;
    }

    public void AddMorale(int moraleVal)
    {
        _morale += moraleVal;
    }

    public void IncreaseCrewLevel()
    {
        _crewLevel += 1;
    }

    public void IncreaseXPNeeded()
    {
        _xpNeeded *= 2; // Subject to change
    }

    private void Awake()
    {
        _crewName = RandomName();
        _inCrew = false;
        _crewLevel = 0;
        _currentXp = 0;
        _xpNeeded = 10; // Subject to change
        _preferences = RandomPreferences();
        _crewType = RandomTypes();
        _morale = defaultMorale;
        GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowInfo()
    {
        GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(1, 1, 1);

        GameObject displayName = GameObject.FindGameObjectWithTag("Crew Name");
        displayName.GetComponent<TextMeshProUGUI>().text = _crewName.ToString();

        GameObject displayType = GameObject.FindGameObjectWithTag("Crew Type");
        displayType.GetComponent<TextMeshProUGUI>().text = "the " + _crewType.ToString();

        GameObject morale = GameObject.FindGameObjectWithTag("Morale");
        morale.GetComponent<TextMeshProUGUI>().text = "Morale: " + _morale.ToString();
        GameObject moraleSlider = GameObject.FindGameObjectWithTag("Morale Slider");
        moraleSlider.GetComponent<Slider>().value = _morale;

        GameObject level = GameObject.FindGameObjectWithTag("Level");
        level.GetComponent<TextMeshProUGUI>().text = "Level: " + _crewLevel.ToString();

        GameObject xpSlider = GameObject.FindGameObjectWithTag("XP Slider");
        xpSlider.GetComponent<Slider>().value = _currentXp;
        xpSlider.GetComponent<Slider>().maxValue = _xpNeeded;
    }

    public void HideInfo()
    {
        GameObject.FindGameObjectWithTag("Info Panel").transform.localScale = new Vector3(0, 0, 0);
    }

    public string CrewName { get { return _crewName; } set => _crewName = value; }
    public string CrewType { get { return _crewType; } set => _crewType = value; }
    public bool InCrew { get { return _inCrew; } set => _inCrew = value; }
    public int CrewLevel { get { return _crewLevel; } }
    public int XPNeeded { get { return _xpNeeded; } }
    public Preference[] Preferences { get { return _preferences; } set => _preferences = value; }
    public int Morale { get { return _morale; } set => _morale = value; }
}
