using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[VolumeComponentMenu("Limitless Glitch/Custom Texture")]

public class CustomTexV : VolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    public TextureParameter texture = new TextureParameter(null);
    public ClampedFloatParameter fade = new ClampedFloatParameter(1f,0,1);
    public bool IsActive() => (bool)enable;

    public bool IsTileCompatible() => false;

}
