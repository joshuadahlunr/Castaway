using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Loads Tutorial Ending </summary>
/// <author> Dana Conley </author>

public class TutorialEnd : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("TutorialEndScene", LoadSceneMode.Single);
    }
}
