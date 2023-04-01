using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Retro Look Pro/Phosphor")]

public class Phosphor : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	public ClampedFloatParameter width = new ClampedFloatParameter(0.4f, 0f, 20f);
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}