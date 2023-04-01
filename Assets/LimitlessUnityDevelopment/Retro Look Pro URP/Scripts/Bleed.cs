using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public enum BleedMode
{
	NTSCOld3Phase,
	NTSC3Phase,
	NTSC2Phase
}
[Serializable]
public sealed class bleedModeParameter : VolumeParameter<BleedMode> { };
[VolumeComponentMenu("Retro Look Pro/Bleed")]

public class Bleed : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Tooltip("NTSC Bleed modes.")]
	public bleedModeParameter bleedMode = new bleedModeParameter();
	[Tooltip("Bleed Stretch amount.")]
	public FloatParameter bleedAmount = new ClampedFloatParameter(0, 0, 15f);
	[Tooltip("Debug bleed curve.")]
	public BoolParameter bleedDebug = new BoolParameter(false);
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter(null);
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

	public int bleedModeIndex;
	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}