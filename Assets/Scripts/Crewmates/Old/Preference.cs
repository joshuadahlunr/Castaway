using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

/// <summary>
/// </summary>
/// <author>Misha Desear</author>
public class Preference
{
    private string _prefName;
    private int _modifier;
    private bool _likeDislike; // where true = like and false = dislike

    private string[] prefNames = { "test", "test2" };
    private bool randomPref = false;

    public Preference()
    {
        _prefName = prefNames[Random.Range(0, prefNames.Length)]; 
        _modifier = RandomModifier(); 
        _likeDislike = LikeOrDislike();
    }

    public int RandomModifier()
    {
        int modifierNum = Random.Range(5, 50);
        return modifierNum;
    }

    public bool LikeOrDislike()
    {
       if (randomPref == false)
        {
            randomPref = true;
        } 
       else
        {
            randomPref = false;
        }
       return randomPref;
    }

    public string PrefName { get { return _prefName; } }
    public int Modifier { get { return _modifier; } }
    public bool LikeDislike { get { return _likeDislike; } }
}
