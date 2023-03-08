using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// </summary>
/// <author>Dana Conley</author>
/// </summary>
namespace EncounterMap
{
    public class PlayerMovementTutorial : MonoBehaviour, IPointerClickHandler
    {
        private Vector2 startPos;
        private Vector2 mousePos;
        private Vector2 newPos;

        void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            mousePos = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out hit))
                {
                    newPos = hit.point;
                    transform.position = newPos;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked");
            PopUps pu = gameObject.GetComponent<PopUps>();
            pu.Update();
        }
    }
}