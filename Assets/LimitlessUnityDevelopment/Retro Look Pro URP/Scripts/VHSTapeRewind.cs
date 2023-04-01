using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Retro Look Pro/VHS Tape Rewind")]

public class VHSTapeRewind : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);

	public ClampedFloatParameter intencity = new ClampedFloatParameter(0.57f, 0, 5);
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0, 1);

	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;

}
