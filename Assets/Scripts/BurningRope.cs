using System;
using UnityEngine;

public class BurningRope : ProgressBar {
	[SerializeField] protected TMPro.TMP_Text text;

	public float current;
	public float max = 1;

	protected new void Update() {
		text.text = $"{(int)current} sec";
		progress = current / max;

		base.Update();
	}
}
