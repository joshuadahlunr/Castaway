using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Component which manages a burning countdown rope (inherits from a progress bar)
/// </summary>
/// <author>Joshua Dahl</author>
public class BurningRope : ProgressBar {
	/// <summary>
	///     Reference to the text above the rope
	/// </summary>
	[SerializeField] protected TMP_Text text;

	[SerializeField] protected RectTransform fire;

	private float delta;

	/// <summary>
	///     Current value
	/// </summary>
	public float current;

	/// <summary>
	///     Maximum value
	/// </summary>
	public float max = 1;

	void Awake() {
		delta = fire.anchorMax.y - fire.anchorMin.y;
	}

	/// <summary>
	///     Every frame update the visualization
	/// </summary>
	protected new void Update() {
		text.text = $"{(int)current} sec";
		progress = current / max;

		progressImage.fillAmount = Mathf.Lerp(.2f, .95f, progress);

		var anchor = fire.anchorMin;
		anchor.y = 1 - progress;
		fire.anchorMin = anchor;
		anchor = fire.anchorMax;
		anchor.y = (1 - progress) + delta;
		fire.anchorMax = anchor;


		// base.Update();
	}
}