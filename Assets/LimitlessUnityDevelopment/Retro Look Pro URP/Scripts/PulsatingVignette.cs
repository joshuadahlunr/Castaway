using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;


[VolumeComponentMenu("Retro Look Pro/Pulsating Vignette")]

public class PulsatingVignette : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Range(0.001f, 50f), Tooltip("Vignette shake speed.")]
	public ClampedFloatParameter speed = new ClampedFloatParameter(1f, 0.001f, 50f);
	[Range(0.001f, 50f), Tooltip("Vignette amount.")]
	public ClampedFloatParameter amount = new ClampedFloatParameter(1f, 0.001f, 50f);

	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}