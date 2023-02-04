using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// @author: Misha Desear
/// </summary>
/// 

[System.Serializable]
public class Crewmate : MonoBehaviour
{
    [SerializeField]
    private int _crewLevel, _xpNeeded, _morale;
    [SerializeField]
    private string _crewName;
    [SerializeField]
    private bool _inCrew;
    [SerializeField]
    private Preference[] _preferences;
    //TODO: implement card type, likes, and dislikes (as class objects)

    private int defaultMorale = 50; // Between a range of 0-100?
    private string[] randomNames = { "Jeff", "Tommy", "David" };

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
        _xpNeeded = 10; // Subject to change
        _preferences = RandomPreferences();
        _morale = defaultMorale;
    }

    public string CrewName { get { return _crewName; } set => _crewName = value; }
    public bool InCrew { get { return _inCrew; } set => _inCrew = value; }
    public int CrewLevel { get { return _crewLevel; } }
    public int XPNeeded { get { return _xpNeeded; } }
    public Preference[] Preferences { get { return _preferences; } set => _preferences = value; }
    public int Morale { get { return _morale; } set => _morale = value; }
}
