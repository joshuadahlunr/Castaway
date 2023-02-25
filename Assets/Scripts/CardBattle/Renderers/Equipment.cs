using System.Collections.Generic;
using System.Linq;
using Extensions;
using GK;
using UnityEngine;

namespace CardBattle.Card.Renderers {
	public class Equipment : Health {
		public LineRenderer arc;
		public CollidingObjects arcCollider;
		private float currentAngle = -1;

		public new void Update() {
			base.Update();
			
			// Only render the arc while the card is in play!
			arc.enabled = (card.state & CardBase.State.InPlay) > 0;
			
			if (card is EquipmentCardBase eCard) {
				if (currentAngle == eCard.arc) return;

				var arcPoints = GetArcPoints(eCard);
				arc.positionCount = arcPoints.Length;
				arc.SetPositions(arcPoints);
				currentAngle = eCard.arc;

				List<Vector3> meshPoints = arcPoints.Select(pos => new Vector3(pos.x, 100, pos.z)).ToList();
				meshPoints.AddRange(arcPoints.Select(pos => new Vector3(pos.x, -200, pos.z)));
				ConvexHullCalculator calculator = new();
				var mesh = calculator.GenerateHullMesh(meshPoints);
				arcCollider.GetComponent<MeshCollider>().sharedMesh = mesh;
			}
		}

		public static Vector3[] GetArcPoints(EquipmentCardBase eCard) {
			const float radius = 10;
			const float segments = 64; // TODO: Set based on graphics quality
			
			// From: https://gamedev.stackexchange.com/questions/128432/how-to-draw-an-arc-between-two-angles
			List<Vector3> arcPoints = new List<Vector3>{ Vector3.zero };
			float angle = 0 - eCard.arc / 2;
			for (var i = 0; i <= segments; i++) {
				var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
				var z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

				arcPoints.Add(new Vector3(x, 0, z));

				angle += eCard.arc / segments;
			}
			arcPoints.Add(Vector3.zero);
			return arcPoints.ToArray();
		}
	}
}