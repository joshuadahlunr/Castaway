using System.Collections.Generic;

namespace CardBattle.Card.Renderers {
	/// <summary>
	/// Extension to the base renderer which adds health text
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Health: Base {
		/// <summary>
		/// Reference to the health UI text
		/// </summary>
		public TMPro.TMP_Text health;

		/// <summary>
		/// Override of update which makes sure the health text is properly updated
		/// </summary>
		public new void Update() {
			base.Update();

			if (card is HealthCardBase hCard)
				health.text = "" + hCard.healthState;
		}

		/// <summary>
		/// Override which also adds in "{health}" as a replaceable placeholder
		/// </summary>
		/// <returns>Dictionary mapping from replacement string to its replacement value</returns>
		protected override Dictionary<string, object> CalculateReplacementParameters() {
			var parameters = base.CalculateReplacementParameters();
			if (card is HealthCardBase hCard) parameters["{health}"] = hCard.healthState;
			return parameters;
		}
	}
}