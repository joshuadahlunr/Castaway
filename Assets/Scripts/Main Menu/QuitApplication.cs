using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    public void Quit() {
        Application.Quit();
        Debug.Log("Application stopped.");
    }
}