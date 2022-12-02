using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Card.Renderers {
	// Class which simply holds references to all of the world space UI elements that can be tweaked
	[RequireComponent(typeof(Canvas))]
	public class Base : MonoBehaviour {
		public Card.CardBase card; // Set automatically
		
		public new TMPro.TMP_Text name;
		public TMPro.TMP_Text cost;
		public Image artwork;
		public TMPro.TMP_Text rules;

		public void Update() {
			name.text = card.name;
			// cost.text = card.cost;
			artwork.sprite = card.art;
			FormatRules(card.rules);
		}

		protected virtual Dictionary<string, object> CalculateReplacementParameters() {
			var parameters = new Dictionary<string, object> {
				{ "{name}", card.name },
				{ "{cost}", card.cost }
			};
			foreach (var (name, prop) in card.properties) 
				parameters.Add($"{{properties[\"{name}\"]}}", prop.value);
			return parameters;
		}

		public void FormatRules(string rulesDescription) {
			// Update the displayed text
			rules.text = CalculateReplacementParameters()
				.Aggregate(rulesDescription, (current, parameter) 
					=> current.Replace(parameter.Key,parameter.Value.ToString()));
		}
	}
}