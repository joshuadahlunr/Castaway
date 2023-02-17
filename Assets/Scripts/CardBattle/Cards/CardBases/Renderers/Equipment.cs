using System.Collections.Generic;
using UnityEngine;

namespace CardBattle.Card.Renderers {
	public class Equipment : Health {
		public LineRenderer arc;
		private float currentAngle = -1;

		public new void Update() {
			const float radius = 10;
			const float segments = 64; // TODO: Set based on graphics quality

			base.Update();
			
			// Only render the arc while the card is in play!
			arc.enabled = (card.state & CardBase.State.InPlay) > 0;
			
			if (card is EquipmentCardBase eCard) {
				if (currentAngle == eCard.arc) return;
				
				// From: https://gamedev.stackexchange.com/questions/128432/how-to-draw-an-arc-between-two-angles
				List<Vector3> arcPoints = new List<Vector3>{ Vector3.zero };
				float angle = 0 - eCard.arc / 2;
				for (var i = 0; i <= segments; i++) {
					var x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
					var y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

					arcPoints.Add(new Vector3(x, 0, y));

					angle += eCard.arc / segments;
				}
				arcPoints.Add(Vector3.zero);

				arc.positionCount = arcPoints.Count;
				arc.SetPositions(arcPoints.ToArray());
				currentAngle = eCard.arc;
			}
		}
	}
}