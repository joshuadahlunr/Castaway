using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component which represents a progress bar
/// </summary>
/// <author>Joshua Dahl</author>
public class ProgressBar : MonoBehaviour {
	/// <summary>
	/// The progress along the bar (0 = 0%, 1 = 100%)
	/// </summary>
	public float progress = 1;
	/// <summary>
	/// Image that is drawn across the bar
	/// </summary>
	[SerializeField] protected Image progressImage;
	
	/// <summary>
	/// Every frame update the bar
	/// </summary>
	protected void Update() {
		if (float.IsNaN(progress))
			progress = 0; // Ensure that Nans don't break everything!
		
		// Scale the image appropriately
		progressImage.rectTransform.anchorMin = new Vector2(progressImage.rectTransform.anchorMin.x, Mathf.Clamp01(1 - progress));
	}
}