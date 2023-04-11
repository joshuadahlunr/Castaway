using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Loads Actual Battle After Intro </summary>
/// <author> Dana Conley </author>

public class ToRealBattle : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("TutorialBattleScene", LoadSceneMode.Single);
    }
}
