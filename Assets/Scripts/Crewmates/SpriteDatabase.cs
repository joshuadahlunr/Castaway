using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

/// <summary>
///     Scriptable Object holding references to textures for loading sprites
/// </summary>
/// <author>Misha Desear</author>
[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "ScriptableObjects/SpriteDatabase")]
public class SpriteDatabase : ScriptableObject
{
    [Serializable]
    public class SpriteDictionary : SerializableDictionaryBase<int, Texture2D> {
    }

    public SpriteDictionary sprites = new();
}
