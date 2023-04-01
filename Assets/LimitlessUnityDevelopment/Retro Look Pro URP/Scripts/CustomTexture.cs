using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Retro Look Pro/Custom Texture")]

public class CustomTexture : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
	[Tooltip("Сustom texture.")]
	public TextureParameter texture = new TextureParameter(null);
	[Range(0f, 1f), Tooltip("Passthrough custom texture alpha chanel.")]
	public BoolParameter alpha = new BoolParameter(true);
	[Range(0f, 1f), Tooltip("fade parameter.")]
	public ClampedFloatParameter fade = new ClampedFloatParameter(1f, 0f, 1f);

	public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;
}