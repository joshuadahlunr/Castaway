using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// </summary>
/// <author>Joshua Dahl</author>

public class MainMenu : MonoBehaviour {
    public TMPro.TMP_Text @new, @continue;

    public void Update() {
        if (DatabaseManager.SaveExists) {
            @new.SetText("New");
            @continue.transform.parent.gameObject.SetActive(true);
        } else {
            @new.SetText("Play");
            @continue.transform.parent.gameObject.SetActive(false);
        }
    }

    public void NewGame() {
        DatabaseManager.ResetToNewSave();
        SceneManager.LoadScene("EncounterMapScene");
    }

    /* TO DOs:
        - Finish settings
        - Finish credits page
        - Get button sprite change to work
    */
}
