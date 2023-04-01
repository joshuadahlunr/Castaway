using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Retro Look Pro/Glitch2")]

public class LimitlessGlitch2 : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    [Range(0f, 2f), Tooltip("Effect speed.")]
    public ClampedFloatParameter speed = new ClampedFloatParameter (1f,0f, 2f);
    [Range(1f, -1f), Tooltip("Effect intensity.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0.1f, 0f, 1f);
    [Range(1f, 2f), Tooltip("block size (higher value = smaller blocks.")]
    public ClampedFloatParameter resolutionMultiplier = new ClampedFloatParameter(1f, 1f, 2f);

    [Range(0f, 1f), Tooltip("blocks width (max value makes effect fullscreen).")]
    public ClampedFloatParameter stretchMultiplier = new ClampedFloatParameter(0.88f,0f, 1f);
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter(null);
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}