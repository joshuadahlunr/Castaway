using System.Collections.Generic;
using UnityEngine.UI;

namespace CardBattle.Card.Renderers {
	/// <summary>
	///     Extension to the base renderer which adds costs
	/// </summary>
	/// <author>Joshua Dahl</author>
	public class Action : Base {
		/// <summary>
		///     Mapping from PeopleJuice types to icons
		/// </summary>
		public PeopleJuice peopleJuiceMapping;

		/// <summary>
		///     References to all of the red background circles that could be visible
		/// </summary>
		public Image[] redCircles;

		/// <summary>
		///     References to all of the people juice cost icons that could be visible
		/// </summary>
		public Image[] costTypes;

		/// <summary>
		///     Override of update which makes sure the cost icons are properly updated
		/// </summary>
		public new void Update() {
			base.Update();

			// If the card is an action card...
			if (card is ActionCardBase aCard) {
				// Display and update all of the icons associated with the card's cost
				for (var i = 0; i < aCard.cost.Count; i++) {
					redCircles[i].gameObject.SetActive(true);
					costTypes[i].sprite =
						peopleJuiceMapping.sprites
							[aCard.cost[i]]; // TODO: Is this lookup to costly to perform every frame?
				}

				// Hide any icons not associated with the card's cost!
				for (var i = aCard.cost.Count; i < redCircles.Length; i++)
					redCircles[i].gameObject.SetActive(false);
			}
		}

		/// <summary>
		///     Override which also adds in "{cost}" as a replaceable placeholder
		/// </summary>
		/// <returns>Dictionary mapping from replacement string to its replacement value</returns>
		protected override Dictionary<string, object> CalculateReplacementParameters() {
			var parameters = base.CalculateReplacementParameters();
			if (card is ActionCardBase aCard) parameters["{cost}"] = aCard.cost;
			return parameters;
		}
	}
}