using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Retro Look Pro/Cinematic Bars")]

public class CinematicBars : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0.5f, 0.01f), Tooltip("Black bars amount (width)")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(0.5f, 0.01f, 0.51f);
	[Range(0f, 1f), Tooltip("Fade black bars.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}