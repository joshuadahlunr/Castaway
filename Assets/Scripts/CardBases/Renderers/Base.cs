using UnityEngine;
using UnityEngine.UI;

namespace Card.Renderers {
	// Class which simply holds references to all of the world space UI elements that can be tweaked
	[RequireComponent(typeof(Canvas))]
	public class Base : MonoBehaviour {
		public new TMPro.TMP_Text name;
		public TMPro.TMP_Text cost;
		public Image artwork;
		public TMPro.TMP_Text rules;
	}
}