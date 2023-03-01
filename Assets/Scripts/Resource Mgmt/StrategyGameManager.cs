using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// </summary>
/// <author>Misha Desear</author>

public class StrategyGameManager : MonoBehaviour
{
    public Slider[] sliderArr;
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
        Globals.SHIP_RESOURCE.AddAmount(-crewSlider.value);
        remainder = (int)crewSlider.value % GlobalCrew.CREW.Count; // Find remainder of slider value divided by crewmates
        evenNum = (int)crewSlider.value - remainder; // Subtract remainder from slider value to get evenly divisible number
        quotient = evenNum / GlobalCrew.CREW.Count;

        for (int i = 0; i < GlobalCrew.CREW.Count; i++)
        {
             GlobalCrew.CREW[i].AddMorale(quotient); // Add quotient from each crewmate's hunger

            if (remainder != 0) // For allocating the remainder of resources
            {
                GlobalCrew.CREW[i].AddMorale(1);
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
