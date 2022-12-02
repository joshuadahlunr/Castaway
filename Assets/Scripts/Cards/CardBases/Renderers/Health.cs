using System.Collections.Generic;

namespace Card.Renderers {
	public class Health: Base {
		public TMPro.TMP_Text health;

		public new void Update() {
			base.Update();

			if (card is HealthCardBase hCard)
				health.text = "" + hCard.health;
		}

		protected override Dictionary<string, object> CalculateReplacementParameters() {
			var parameters = base.CalculateReplacementParameters();
			if (card is HealthCardBase hCard) parameters["{health}"] = hCard.health;
			return parameters;
		}
	}
}