using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Loads Tutorial Map after battle </summary>
/// <author> Dana Conley </author>

public class BackToMap : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("TutorialMapScene2", LoadSceneMode.Single);
    }
}
