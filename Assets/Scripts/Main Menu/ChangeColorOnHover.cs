using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// </summary>
/// <author>Jared White</author>

public class ChangeColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    TextMeshProUGUI text;
    Image thisImage;
    public Sprite banner;

    void Start() {
        text = GetComponent<TextMeshProUGUI>();
        thisImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData){
        text.color = Color.white;
        //thisImage.sprite = banner;
    }

    public void OnPointerExit(PointerEventData pointerEventData){
        text.color = new Color32(64, 34, 34, 255);
        //thisImage.sprite = null;
    }
}