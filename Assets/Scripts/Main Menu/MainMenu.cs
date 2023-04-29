using System;
using System.Collections;
using System.Collections.Generic;
using ResourceMgmt;
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

        // The player starts every new game with 10 HP
        var shipUpgradeInfo = DatabaseManager.GetOrCreateTable<ResourceManager.UpgradeInfo>().FirstOrDefault();
        shipUpgradeInfo.currentShipHealth = 10;
        DatabaseManager.database.InsertOrReplace(shipUpgradeInfo);

        EncounterMap.PlayerMovement.ResetPosition();
        EncounterMap.CameraMovement.ResetCamPosition();
        SceneManager.LoadScene("EncounterMapScene");
    }

    /* TO DOs:
        - Finish settings
        - Finish credits page
        - Get button sprite change to work
    */
}
