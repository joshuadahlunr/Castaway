using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipManager : MonoBehaviour
{
    public Slider[] sliderArr;
    public Button confirmBtn;
    public void DoLevelUp()
    {
        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            if (Globals.UPGRADE_DATA[i].Progress >= Globals.UPGRADE_DATA[i].Cost)
            {
                Globals.UPGRADE_DATA[i].IncreaseCost();
                Globals.UPGRADE_DATA[i].IncreaseLevel();
                Globals.UPGRADE_DATA[i].ResetProgress();
                sliderArr[i].maxValue = Globals.UPGRADE_DATA[i].Cost;
                Debug.Log("For " + Globals.UPGRADE_DATA[i].Code + ", new cost is " + Globals.UPGRADE_DATA[i].Cost.ToString() + " and level is " + Globals.UPGRADE_DATA[i].Level.ToString());
            }
        }
    }

    private void Awake()
    {
        confirmBtn.onClick.AddListener(DoLevelUp);
    }
}
