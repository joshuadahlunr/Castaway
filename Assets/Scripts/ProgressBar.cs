using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
	public float progress = 1;
	[SerializeField] protected Image progressImage;
	
	protected void Update() {
		if (float.IsNaN(progress))
			progress = 0;
		progressImage.rectTransform.anchorMin = new Vector2(progressImage.rectTransform.anchorMin.x, Mathf.Clamp01(1 - progress));
	}
}