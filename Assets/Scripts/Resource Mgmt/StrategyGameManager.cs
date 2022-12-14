using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// @author: Misha Desear
/// </summary>

public class StrategyGameManager : MonoBehaviour
{
    public Slider[] sliderArr;
    public Crewmate[] crew;
    public Slider crewSlider;
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
        // TODO: fix so crewmates cannot be "overfed" past 0 hunger
        remainder = (int)crewSlider.value % crew.Length; // Find remainder of slider value divided by crewmates
        evenNum = (int)crewSlider.value - remainder; // Subtract remainder from slider value to get evenly divisible number
        quotient = evenNum / crew.Length;

        for (int i = 0; i < crew.Length; i++)
        {
 
            crew[i].AddHunger(-quotient); // Subtract quotient from each crewmate's hunger

            if (remainder != 0) // For allocating the remainder of resources
            {
                crew[i].AddHunger(1);
                remainder -= 1;
            }
        }
    }

    private void Awake()
    {
        sustainBtn.onClick.AddListener(SustainShip);
        clearBtn.onClick.AddListener(ClearSliders);

        AddResources();

        winMsg.text = "Congratulations! You have earned " + resourcesWon.ToString() + " resources for your ship and crew. You have " +
            Globals.SHIP_RESOURCE.Amount.ToString() + " resources in total. How would you like to allocate them?";
    }
}
