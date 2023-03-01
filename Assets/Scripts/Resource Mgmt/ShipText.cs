using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipText : MonoBehaviour
{
    public TextMeshProUGUI[] textArr;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < textArr.Length; i++)
        {
            textArr[i].text = Globals.UPGRADE_DATA[i].Code.ToString() + ": Level " + Globals.UPGRADE_DATA[i].Level.ToString();
        }
    }
}
