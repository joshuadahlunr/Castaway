using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class UpgradeData
{
    private string _code;
    private float _cost;
    private float _progress;
    private float _level;
    public UpgradeData(string code, float cost, float progress, float level)
    {
        _code = code; // To refer to a certain part of the ship
        _cost = cost; // How many resources are needed for an upgrade
        _progress = progress; // Progress made towards upgrade
        _level = level; // Current upgrade level
    }

    public void AddProgress(float value)
    {
        _progress += value; 
    }

    public void ResetProgress()
    {
        // If progress is equal to cost, reset progress to 0
        if (_progress == _cost)
        {
            _progress = 0;

        } 
        
        // Otherwise, set progress to difference of progress and cost
        else if (_progress > _cost)
        {
            _progress = _progress - _cost;
        }
    }

    public void IncreaseCost()
    {
        _cost *= 2;
    }

    public void IncreaseLevel()
    {
        _level += 1;
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
    public float Level { get => _level; }
}
