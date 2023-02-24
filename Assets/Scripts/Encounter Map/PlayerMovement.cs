using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// </summary>
/// <author>Jared White</author>
/// </summary>

public class PlayerMovement : MonoBehaviour, IPointerClickHandler
{
    private Vector2 startPos;
    private Vector2 mousePos;
    private Vector2 newPos;

    void Start() {
        startPos = transform.position;
    }

    void Update() {
        mousePos = Mouse.current.position.ReadValue();
        if (Mouse.current.leftButton.wasPressedThisFrame){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out hit)){
                newPos = hit.point;
                transform.position = newPos;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        Debug.Log("Clicked");
    }

    /* Figure out how to get tweening or lerp to work in order to follow
    the time it would take for the player to traverse between destinations.
    Figure out how to not get the scene switch to be instantaneous
    */
    /* void Update() {
       if (OnClick()) {
            Ray click = Camera.main.ScreenPointToRay(mousePositionAction);
            RaycastHit hit;
            if (Physics.Raycast(click, out hit)) {
                if (hit.collider == GetComponent<Collider>()) {
                    moveTowards = !moveTowards;
                }
            }
        }
    }

    private void Awake() {
        newPos = transform.position;
        //leftClick = new InputAction(binding: "<Mouse>/leftButton");
    }

    private void Move() {
        Vector3 startPos = transform.position;

        if(Mouse.current.leftButton.wasPressedThisFrame){
            var mousePos = pointerAction.action.ReadValue<Vector2>.();
        }

        transform.position = Vector2.Lerp (startPos, mousePos, Time.deltaTime);
    }

    private void OnEnable() {
        leftClick.action.Enable();
        mousePositionAction.action.Enable();

        leftClick.action.performed += ctx => OnClick;
    }

    private void OnClick(InputAction.CallBackContext ctx) {
		Move();
	}*/
}