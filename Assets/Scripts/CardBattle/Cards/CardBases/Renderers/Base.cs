using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Card.Renderers {
	/// <summary>
	/// Component which simply holds references to all of the world space UI elements that can be tweaked
	/// </summary>
	/// <author>Joshua Dahl</author>
	[RequireComponent(typeof(Canvas))]
	public class Base : MonoBehaviour {
		/// <summary>
		/// The card which created us
		/// </summary>
		/// <remarks>Set automatically</remarks>
		public Card.CardBase card; // Set automatically
		
		/// <summary>
		/// Reference to the name UI text
		/// </summary>
		public new TMPro.TMP_Text name;
		/// <summary>
		/// Reference to the UI artwork image
		/// </summary>
		public Image artwork;
		/// <summary>
		/// Reference to the rules UI text
		/// </summary>
		public TMPro.TMP_Text rules;

		/// <summary>
		/// Every frame make sure the text we are displaying matches the state of the card
		/// </summary>
		public void Update() {
			name.text = card.name;
			artwork.sprite = card.art;
			FormatRules(card.rules);
		}

		
		/// <summary>
		/// Function that gathers all of the properties that can be used as placeholders in the card's description
		/// </summary>
		/// <returns>Dictionary mapping from replacement string to its replacement value</returns>
		protected virtual Dictionary<string, object> CalculateReplacementParameters() {
			var parameters = new Dictionary<string, object> {
				{ "{name}", card.name },
			};
			foreach (var (name, prop) in card.properties) 
				parameters.Add($"{{properties[\"{name}\"]}}", prop.value);
			return parameters;
		}

		/// <summary>
		///  Function which formats the rules string, applying any placeholder replacements
		/// </summary>
		/// <param name="rulesDescription">The rules text which needs to be displayed/have placeholders replaced</param>
		protected void FormatRules(string rulesDescription) {
			// Update the displayed text
			rules.text = CalculateReplacementParameters()
				.Aggregate(rulesDescription, (current, parameter) 
					=> current.Replace(parameter.Key,parameter.Value.ToString()));
		}
	}
}