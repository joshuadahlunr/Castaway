using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// @author: Jared White
/// </summary>

public class ChangeColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;

    void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData){
        text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData pointerEventData){
        text.color = Color.black;//new Color(64, 34, 34, 255);
    }
}