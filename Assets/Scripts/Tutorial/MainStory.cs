using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Loads Tutorial Intro </summary>
/// <author> Dana Conley </author>

public class MainStory : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("TutorialMapScene", LoadSceneMode.Single);
    }
}
