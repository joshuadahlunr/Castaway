using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class ShipResource
{
    private float _currentAmt;

    public ShipResource(float initialAmt)
    {
        _currentAmt = initialAmt;
    }
    
    public void AddAmount(float value)
    {
        _currentAmt += value;
        if (_currentAmt < 0) _currentAmt = 0;
    }
    public float Amount { get => _currentAmt; }
}
