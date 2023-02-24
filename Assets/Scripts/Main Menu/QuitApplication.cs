using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// </summary>
/// <author>Jared White</author>

public class QuitApplication : MonoBehaviour
{
    public void Quit() {
        Application.Quit();
        Debug.Log("Application stopped.");
    }
}