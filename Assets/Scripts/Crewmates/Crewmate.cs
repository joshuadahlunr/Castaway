using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class Crewmate : MonoBehaviour
{
    [SerializeField]
    private string _id;
    private string _crewName;
    private bool _inCrew;
    private int _crewLevel;
    private int _xpNeeded;
    private int _morale;
    //TODO: implement card type, likes, and dislikes (as class objects)

    public Crewmate(string id, string crewName, bool inCrew, int crewLevel, int xpNeeded, int morale)
    {
        _id = id;
        _crewName = crewName;
        _inCrew = inCrew;
        _crewLevel = crewLevel;
        _xpNeeded = xpNeeded;
        _morale = morale;
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

    public string ID { get => _id; }
    public string CrewName { get => _crewName; }
    public bool InCrew { get => _inCrew; }
    public int CrewLevel { get => _crewLevel; }
    public int XPNeeded { get => _xpNeeded; }
    public int Morale { get => _morale;}
}
