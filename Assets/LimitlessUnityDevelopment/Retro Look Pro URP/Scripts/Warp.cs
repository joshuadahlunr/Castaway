﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Retro Look Pro/Warp")]

public class Warp : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0f, 1f), Tooltip("Dark areas adjustment.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	[Tooltip("Warp mode.")]
	public WarpModeParameter warpMode = new WarpModeParameter();
	[Tooltip("Warp image corners on x/y axes.")]
	public Vector2Parameter warp = new Vector2Parameter(new Vector2(0.03125f, 0.04166f));
	[Tooltip("Warp picture center.")]
	public FloatParameter scale = new FloatParameter(1f);
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}