using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <author>Jared White</author>
/// </summary>
namespace EncounterMap {
    /// This is used to ensure that the background follows the camera's movement.
    /// This ensures that the player is always within the map and never out of its bounds.
    public class BackgroundMovement : MonoBehaviour {
        // Variables for the camera's x and y positions
        private float camX, camY;

        void Update() {
            // Get the camera's x and y positions
            camX = Camera.main.transform.position.x;
            camY = Camera.main.transform.position.y;

            // Move the background to the new position based off the camera's x and y position
            transform.position = new Vector3(camX, camY, transform.position.z);
        }
    }
}