using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class Preference : MonoBehaviour
{
    private string _prefName;
    private int _modifier;
    private bool _likeDislike; // where 0 = like and 1 = dislike

    public Preference(string prefName, int modifier, bool likeDislike)
    {
        _prefName = prefName; 
        _modifier = modifier; 
        _likeDislike = likeDislike;
    }

    public string PrefName { get { return _prefName; } set => _prefName = value; }
    public int Modifier { get => _modifier; }
    public bool LikeDislike { get { return _likeDislike; } set => _likeDislike = value; }
}
