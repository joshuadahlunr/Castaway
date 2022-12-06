using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData
{
    private string _code;
    private float _cost;
    private float _progress;
    public UpgradeData(string code, float cost, float progress)
    {
        _code = code; // To refer to a certain part of the ship
        _cost = cost; // How many resources are needed for an upgrade
        _progress = progress; // Progress made towards upgrade
    }

    public void AddProgress(float value)
    {
        _progress += value; 
    }

    public bool CanBuy()
    {
        if (Globals.SHIP_RESOURCE.Amount < _cost)
        {
            return false;
        }
        return true;
    }

    public string Code { get => _code; }
    public float Cost { get => _cost; }
    public float Progress { get => _progress; }
}
