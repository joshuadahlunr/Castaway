using UnityEngine;

/// <summary>
/// Component which manages a burning countdown rope (inherits from a progress bar)
/// </summary>
/// <author>Joshua Dahl</author>
public class BurningRope : ProgressBar {
	/// <summary>
	/// Reference to the text above the rope
	/// </summary>
	[SerializeField] protected TMPro.TMP_Text text;

	/// <summary>
	/// Current value
	/// </summary>
	public float current;
	/// <summary>
	/// Maximum value
	/// </summary>
	public float max = 1;

	/// <summary>
	/// Every frame update the visualization
	/// </summary>
	protected new void Update() {
		text.text = $"{(int)current} sec";
		progress = current / max;

		base.Update();
	}
}
