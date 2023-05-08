using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <author>Jared White</author>
/// </summary>
namespace EncounterMap {
    /// <summary>
    /// This script handles the player movement as well as the player's position throughout the map
    /// </summary>
    public class PlayerMovement : MonoBehaviour {
        // Initialize the static variable to be the player's start position
        public static Vector3 playerPos = new Vector3(-15f, 0f, 90.0f);
        public Animator animator;
        public EncounterMapScript map;
        
        [SerializeField]
        private float speed = 8f;
        [SerializeField] float nodeGravity = 3;
        public bool moving = false;
        private float vertical, horizontal;

        private Camera mainCamera;
        private Rigidbody2D playerRB;
        private BoxCollider2D playerCol;
        private Vector2 movementInput = new Vector2(1, 0);

        void Awake() {
            // Get the 2D Rigidbody of the player gameObject
            playerRB = GetComponent<Rigidbody2D>();
        }

        void Start() {
            // Set the player's new position to the one passed from the MapNode script
            transform.position = playerPos;
        }

        void FixedUpdate() {
            // This handles the player movement and sets the animation bool accordingly to switch between idle and movement animations
            playerPos = transform.position;
            var velocity =  new Vector3(movementInput.x, movementInput.y, 0) * speed;
            velocity += (map.ClosestNode(playerPos).transform.position - playerPos).normalized * nodeGravity;
            velocity.y -= Mathf.Pow(playerPos.y / 100, 3);
            playerRB.velocity = velocity;
            if(playerRB.velocity == Vector2.zero) {
                moving = false;
            }
            animator.SetBool("Moving", moving);
        }

        // Uses the Unity Input System to get the input value that determines what direction to move in 
        private void OnMove(InputValue inputVal) {
            movementInput = inputVal.Get<Vector2>();
            movementInput.x = 1;
            // Set moving to true
            moving = true;
        }

        // This is used to reset the player to the start position for a new game
        public static void ResetPosition() {
            playerPos = new Vector3(-15f, 0f, 90.0f);
        }
    }
}