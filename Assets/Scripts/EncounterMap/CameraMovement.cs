using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <author>Jared White</author>
/// </summary>
namespace EncounterMap {
    /// This is used for the camera to continuously follow the player
    /// as it moves across the screen 
    public class CameraMovement : MonoBehaviour {
        // The offset between the Camera and Target so the camera stays at the same z value
        public static Vector3 followPlayer;
        private Vector3 camOffset = new Vector3(0f, 0f, -100f);
        private float time = 0.25f;
        private Vector3 velocity = Vector3.zero;
        [SerializeField] private Transform target;

        void Start() {
            // Update position to the player's new position for smoother camera transitions
            transform.position = followPlayer;
        }

        // Update is called once per frame
        void FixedUpdate() {
            // Define the target position and add the camera offset
            Vector3 targetPos = target.position + camOffset;
            // Move the camera towards the target position that we set above
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, time);
        }

        public static void ResetCamPosition() {
            followPlayer = new Vector3(0f, 0f, -10f);
        }
    }
}