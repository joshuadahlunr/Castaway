using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StrategyGameManager : MonoBehaviour
{
    public Slider[] sliderArr;
    public Button confirmBtn;
    public Button sustainBtn;
    public Button clearBtn;

    public float resourcesWon = 50;
    public float upgradeTime = 60;
    public float timeLeftToUpgrade => upgradeTimer;

    public TextMeshProUGUI winMsg;

    private float upgradeTimer = 0;
    public void AddResources()
    {
        Globals.SHIP_RESOURCE.AddAmount(resourcesWon);
    }

    private bool canUpgrade;
    public void Upgrade()
    {

        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            if (Globals.UPGRADE_DATA[i].CanBuy()) // If we have enough resources for an upgrade...
            {
                Globals.SHIP_RESOURCE.AddAmount(-sliderArr[i].value); // ...subtract however much was allocated to slider
                Globals.UPGRADE_DATA[i].AddProgress(sliderArr[i].value); // ...and record progress toward that upgrade
                Debug.Log("Progress towards upgrade " + i.ToString() + " is: " + Globals.UPGRADE_DATA[i].Progress.ToString());
            }
            else
            {
                break;
            }
        }
    }

    /* private float sliderTotals;
    private float currentVal;

    public void ValidateResources()
    {
        sliderTotals = 0;

        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            sliderTotals += sliderArr[i].value;
        }

        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            if (sliderTotals > Globals.SHIP_RESOURCE.Amount)
            {
                sliderArr[i].value = currentVal;
            }
        }
    } */

    private int remainder;
    private int evenNum;
    private int quotient;

    // For dividing resources evenly, with regards to remainder
    public void SustainShip()
    {
        remainder = (int)Globals.SHIP_RESOURCE.Amount % Globals.UPGRADE_DATA.Length; // Find remainder of resources divided by upgrades
        evenNum = (int)Globals.SHIP_RESOURCE.Amount - remainder; // Subtract remainder from resource amount to get evenly divisible number
        quotient = evenNum / Globals.UPGRADE_DATA.Length;

        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            sliderArr[i].value = quotient; // Assign quotient to each slider

            if (remainder != 0) // For allocating the remainder of resources
            {
                sliderArr[i].value += 1;
                remainder -= 1;
            }
        }
    }

    public void ClearSliders()
    {
        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            sliderArr[i].value = 0;
        }
    }
    public void FeedCrew()
    {
        // TODO: implement once crew is prototyped
    }

    private void Awake()
    {
        confirmBtn.onClick.AddListener(Upgrade);
        sustainBtn.onClick.AddListener(SustainShip);
        clearBtn.onClick.AddListener(ClearSliders);

        AddResources();

        /*for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            sliderArr[i].onValueChanged.AddListener(delegate { ValidateResources(); });
        }*/

        winMsg.text = "Congratulations! You have earned " + resourcesWon.ToString() + " resources for your ship and crew. You have " +
            Globals.SHIP_RESOURCE.Amount.ToString() + " resources in total. How would you like to allocate them?";
    }
}
