using System.Collections.Generic;
using System.Linq;
using GK;
using UnityEngine;

namespace CardBattle.Card.Renderers {
    /// <summary>
    ///     Renders an arc for EquipmentCardBase instances.
    /// </summary>
    /// <author>Joshua Dahl</author>
    public class Equipment : Health {
        /// <summary>
        ///     The LineRenderer used to draw the arc.
        /// </summary>
        public LineRenderer arc;

        /// <summary>
        ///     The CollidingObjects used to represent the mesh for the arc.
        /// </summary>
        public CollidingObjects arcCollider;

        /// <summary>
        ///     The current angle of the arc.
        /// </summary>
        private float currentAngle = -1;

        /// <summary>
        ///     Overrides the Update function defined in the base class.
        /// </summary>
        public new void Update() {
			base.Update();

			// Only render the arc while the card is in play!
			arc.enabled = (card.state & CardBase.State.InPlay) > 0;

			if (card is EquipmentCardBase eCard) {
				if (currentAngle == eCard.arc) return;

				// Update the arc with the new eCard.arc value
				var arcPoints = GetArcPoints(eCard);
				arc.positionCount = arcPoints.Length;
				arc.SetPositions(arcPoints);
				currentAngle = eCard.arc;

				// Create a list of Vector3 points to represent the mesh
				var meshPoints = arcPoints.Select(pos => new Vector3(pos.x, 100, pos.z)).ToList();
				meshPoints.AddRange(arcPoints.Select(pos => new Vector3(pos.x, -200, pos.z)));

				// Use a convex hull calculator to generate a mesh from the mesh points and set it as the shared mesh for the arc collider
				ConvexHullCalculator calculator = new();
				var mesh = calculator.GenerateHullMesh(meshPoints);
				arcCollider.GetComponent<MeshCollider>().sharedMesh = mesh;
			}
		}

        /// <summary>
        ///     Gets the arc points for an EquipmentCardBase instance.
        /// </summary>
        /// <param name="eCard">The EquipmentCardBase instance to get the arc points for.</param>
        /// <returns>An array of Vector3 points representing the arc.</returns>
        public static Vector3[] GetArcPoints(EquipmentCardBase eCard) {
			// Define constants for the radius and segment count of the arc
			const float radius = 10;
			const float segments = 64; // TODO: Set based on graphics quality

			// From: https://gamedev.stackexchange.com/questions/128432/how-to-draw-an-arc-between-two-angles

			// Create an empty list to hold the arc points
			var arcPoints = new List<Vector3> { Vector3.zero };

			// Calculate the starting angle of the arc
			var angle = 0 - eCard.arc / 2;

			// Iterate over the number of segments and calculate the x and z coordinates of each point on the arc using trigonometry
			for (var i = 0; i <= segments; i++) {
				var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
				var z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

				arcPoints.Add(new Vector3(x, 0, z));

				angle += eCard.arc / segments;
			}

			// Add the starting point to the beginning and end of the list
			arcPoints.Add(Vector3.zero);

			// Return the array of arc points
			return arcPoints.ToArray();
		}
	}
}