using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using RetroLookPro.Enums;
using LimitlessDev.RetroLookPro;

[Serializable]
public sealed class resModeParameter : VolumeParameter<ResolutionMode> { };
[Serializable]
public sealed class Vector2IntParameter : VolumeParameter<Vector2Int> { };
[Serializable]
public sealed class preLParameter : VolumeParameter<effectPresets> { };

[VolumeComponentMenu("Retro Look Pro/Colormap Palette")]

public class ColormapPalette : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	public IntParameter pixelSize = new IntParameter(240);
	[Range(0f, 1f), Tooltip("Opacity.")]
	public ClampedFloatParameter opacity = new ClampedFloatParameter(1f, 0f, 1f);
	[Range(0f, 1f), Tooltip("Dithering effect.")]
	public ClampedFloatParameter dither = new ClampedFloatParameter(1f, 0f, 1f);
	public preLParameter presetsList = new preLParameter { };

	public IntParameter presetIndex = new IntParameter(0);
	[Tooltip("Dither texture.")]
	public TextureParameter bluenoise = new TextureParameter(null); public bool IsActive() => (bool)enable;
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();


	public bool IsTileCompatible() => false;
}