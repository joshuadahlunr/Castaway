using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderAction : MonoBehaviour
{
    public TextMeshProUGUI sliderText;
    public Slider slider;
    public Slider[] sliderArr;

    // Update is called once per frame
    void Update()
    {
        sliderText.text = slider.value.ToString();
    }

    private void Awake()
    {
        slider.onValueChanged.AddListener(delegate { ValidateResources(); });
    }

    private float sliderTotals;
    private float currentVal;
    public void ValidateResources()
    {
        sliderTotals = 0;
        
        for (int i = 0; i < Globals.UPGRADE_DATA.Length; i++)
        {
            sliderTotals += sliderArr[i].value;
        }

        if (sliderTotals > Globals.SHIP_RESOURCE.Amount)
        {
            slider.value = currentVal;
        }
    }
}
