using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class Crewmate : MonoBehaviour
{
    [SerializeField]
    private int _id;
    private string _crewName;
    private bool _inCrew;
    private int _crewLevel;
    private int _xpNeeded;
    private int _hunger;
    private Preference[] _likes;
    private Preference[] _dislikes;
    private int _morale;
    //TODO: implement card type, likes, and dislikes (as class objects)

    public Crewmate(int id, string crewName, bool inCrew, int crewLevel, int xpNeeded, 
        int hunger, Preference[] likes, Preference[] dislikes, int morale)
    {
        _id = id;
        _crewName = crewName;
        _inCrew = inCrew;
        _crewLevel = crewLevel;
        _xpNeeded = xpNeeded;
        _hunger = hunger;
        _likes = likes;
        _dislikes = dislikes;
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

    public void AddHunger(int hungerVal)
    {
        _hunger += hungerVal;
    }

    public void IncreaseCrewLevel()
    {
        _crewLevel += 1;
    }

    public void IncreaseXPNeeded()
    {
        _xpNeeded *= 2; // Subject to change
    }

    public int ID { get => _id; }
    public string CrewName { get => _crewName; }
    public bool InCrew { get => _inCrew; }
    public int CrewLevel { get => _crewLevel; }
    public int XPNeeded { get => _xpNeeded; }
    public int Hunger { get => _hunger; }
    public Preference[] Likes { get { return _likes; } set => _likes = value; }
    public Preference[] Dislikes { get { return _dislikes; } set => _dislikes = value; }
    public int Morale { get => _morale;}
}
