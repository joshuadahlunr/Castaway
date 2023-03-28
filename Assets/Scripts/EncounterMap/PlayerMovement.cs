using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// </summary>
/// <author>Jared White</author>
/// </summary>
namespace EncounterMap {
    public class PlayerMovement : MonoBehaviour {
        // Set the target node position from the MapNode script
        public static Vector2 targetPos;
        
        private float speed = 2f;
        private bool moving;

        MapNode[] nodes;
        // Used to track player progression
        public int currentNode;

        private Camera mainCamera;
        private Rigidbody2D playerRB;
        private BoxCollider2D playerCol;

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