using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// <author>Jared White</author>
/// </summary>
namespace EncounterMap {
    public class PlayerMovement : MonoBehaviour {
        // Set the target node position from the MapNode script
        public static Vector2 targetPos;

        public Animator animator;
        
        [SerializeField]
        private float speed = 8f;
        public bool moving = false;
        private float vertical, horizontal;

        MapNode[] nodes;
        // Used to track player progression
        public int currentNode;

        private Camera mainCamera;
        private Rigidbody2D playerRB;
        private BoxCollider2D playerCol;
        private Vector2 movementInput;

        void Awake() {
            playerRB = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate() {
            playerRB.velocity = movementInput * speed;
            if(playerRB.velocity == Vector2.zero) {
                moving = false;
            }
            animator.SetBool("Moving", moving);
        }

        private void OnMove(InputValue inputVal) {
            movementInput = inputVal.Get<Vector2>();
            moving = true;
        }

        void OnCollisionEnter2D(Collision2D col) {
            Debug.Log("Collision with event");
        }

    /*    void Start() { 
            // Set the Rigidbody to the player GameObject
            playerRB = GetComponent<Rigidbody2D>();
            // Set the BoxColider to the player GameObject
            playerCol = GetComponent<BoxCollider2D>();
            // Set the Camera to the main camera
            mainCamera = Camera.main;
            // Set the nodes array to get the MapNode components in the parent object (Map)
            nodes = GetComponentsInParent<MapNode>();
        }

        public void Move(InputAction.CallbackContext context) {
            if ((Vector2)transform.position != (Vector2)targetPos) {
                transform.position = Vector2.Lerp((Vector2)transform.position, (Vector2)targetPos, Time.deltaTime);
            }
        }

        public void CheckNode() {
            
        }

      public void Move(InputAction.CallbackContext context) {
            //playerRB.velocity = Vector2.zero;
            //Vector2 direction = (Vector2)targetPos;
            //direction.Normalize();
            if (targetPos != null) {
                if (coroutine != null) {
                    StopCoroutine(coroutine);
                }

                coroutine = StartCoroutine(PlayerMoveTowards((Vector2)targetPos));
            }
        }

        public IEnumerator PlayerMoveTowards (Vector2 target) {
            Vector2 distance = target - playerRB.position;
            while (distance.sqrMagnitude > float.Epsilon) {
                Vector2 newPos = Vector2.MoveTowards(transform.position, target, Time.deltaTime*speed);
                playerRB.MovePosition(newPos);
                yield return null;
            }
        }

        public void Move(InputAction.CallbackContext context) {
             float distance = Vector3.Distance(transform.position, target);
            while (distance > float.Epsilon) {
                Vector3 endPos = Vector3.MoveTowards(playerRB.position, target, speed * Time.deltaTime);
                playerRB.MovePosition(endPos);
                distance = Vector3.Distance(transform.position, target);

                Vector3 direction = target - transform.position;
                Vector3 movement = direction.normalized * speed * Time.deltaTime; 

                playerRB.velocity = direction.normalized * speed;

                transform.rotation = Quaternion.LookRotation(direction.normalized);
                yield return null;
            }
        }

        private IEnumerator PlayerMoveTowards(Vector3 target) {
            while (Vector3.Distance(transform.position, target) > 0.1f) {
                Vector3 destination = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                transform.position = destination;
                yield return null;
            }
        }

        */
    }
}