using System;
using UnityEngine;

public enum AnchorPresets {
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottonCenter,
	BottomRight,
	// BottomStretch,

	VertStretchLeft,
	VertStretchRight,
	VertStretchCenter,

	HorStretchTop,
	HorStretchMiddle,
	HorStretchBottom,

	StretchAll
}

public enum PivotPresets {
	TopLeft,
	TopCenter,
	TopRight,

	MiddleLeft,
	MiddleCenter,
	MiddleRight,

	BottomLeft,
	BottomCenter,
	BottomRight
}

public static class RectTransformExtensions {
	public static void SetAnchor(this RectTransform source, AnchorPresets align, int offsetX = 0, int offsetY = 0) {
		source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

		switch (align) {
			case AnchorPresets.TopLeft: {
				source.anchorMin = new Vector2(0, 1);
				source.anchorMax = new Vector2(0, 1);
				break;
			}
			case AnchorPresets.TopCenter: {
				source.anchorMin = new Vector2(0.5f, 1);
				source.anchorMax = new Vector2(0.5f, 1);
				break;
			}
			case AnchorPresets.TopRight: {
				source.anchorMin = new Vector2(1, 1);
				source.anchorMax = new Vector2(1, 1);
				break;
			}

			case AnchorPresets.MiddleLeft: {
				source.anchorMin = new Vector2(0, 0.5f);
				source.anchorMax = new Vector2(0, 0.5f);
				break;
			}
			case AnchorPresets.MiddleCenter: {
				source.anchorMin = new Vector2(0.5f, 0.5f);
				source.anchorMax = new Vector2(0.5f, 0.5f);
				break;
			}
			case AnchorPresets.MiddleRight: {
				source.anchorMin = new Vector2(1, 0.5f);
				source.anchorMax = new Vector2(1, 0.5f);
				break;
			}

			case AnchorPresets.BottomLeft: {
				source.anchorMin = new Vector2(0, 0);
				source.anchorMax = new Vector2(0, 0);
				break;
			}
			case AnchorPresets.BottonCenter: {
				source.anchorMin = new Vector2(0.5f, 0);
				source.anchorMax = new Vector2(0.5f, 0);
				break;
			}
			case AnchorPresets.BottomRight: {
				source.anchorMin = new Vector2(1, 0);
				source.anchorMax = new Vector2(1, 0);
				break;
			}

			case AnchorPresets.HorStretchTop: {
				source.anchorMin = new Vector2(0, 1);
				source.anchorMax = new Vector2(1, 1);
				break;
			}
			case AnchorPresets.HorStretchMiddle: {
				source.anchorMin = new Vector2(0, 0.5f);
				source.anchorMax = new Vector2(1, 0.5f);
				break;
			}
			case AnchorPresets.HorStretchBottom: {
				source.anchorMin = new Vector2(0, 0);
				source.anchorMax = new Vector2(1, 0);
				break;
			}

			case AnchorPresets.VertStretchLeft: {
				source.anchorMin = new Vector2(0, 0);
				source.anchorMax = new Vector2(0, 1);
				break;
			}
			case AnchorPresets.VertStretchCenter: {
				source.anchorMin = new Vector2(0.5f, 0);
				source.anchorMax = new Vector2(0.5f, 1);
				break;
			}
			case AnchorPresets.VertStretchRight: {
				source.anchorMin = new Vector2(1, 0);
				source.anchorMax = new Vector2(1, 1);
				break;
			}

			case AnchorPresets.StretchAll: {
				source.anchorMin = new Vector2(0, 0);
				source.anchorMax = new Vector2(1, 1);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException(nameof(align), align, null);
		}
	}

	public static void SetPivot(this RectTransform source, PivotPresets preset) {
		source.pivot = preset switch {
			PivotPresets.TopLeft => new Vector2(0, 1),
			PivotPresets.TopCenter => new Vector2(0.5f, 1),
			PivotPresets.TopRight => new Vector2(1, 1),
			PivotPresets.MiddleLeft => new Vector2(0, 0.5f),
			PivotPresets.MiddleCenter => new Vector2(0.5f, 0.5f),
			PivotPresets.MiddleRight => new Vector2(1, 0.5f),
			PivotPresets.BottomLeft => new Vector2(0, 0),
			PivotPresets.BottomCenter => new Vector2(0.5f, 0),
			PivotPresets.BottomRight => new Vector2(1, 0),
			_ => source.pivot
		};
	}

	public static void ResetTransformOffset(this RectTransform rectTransform) {
		rectTransform.offsetMin = new Vector2(0, 0);
		rectTransform.offsetMax = new Vector2(0, 0);
	}
}