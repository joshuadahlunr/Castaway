using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using Crew;

/// <summary>
/// Scriptable object holding references to crewmates
/// </summary>
/// <remarks>Converts names in SQL to Unity objects</remarks>
/// <author>Misha Desear</author>

public class CrewDatabase : ScriptableObject
{
    [Serializable]
    public class CrewDictionary : SerializableDictionaryBase<string, Crewmates> {}
    public CrewDictionary crew = new CrewDictionary();

    public Crewmates Instantiate(string crewName)
        => crew.TryGetValue(crewName, out var crewmates) ? Instantiate(crewmates.gameObject).GetComponent<Crewmates>() : null;
    public Crewmates Instantiate(string crewName, Transform parent)
        => crew.TryGetValue(crewName, out var crewmates) ? Instantiate(crewmates.gameObject, parent).GetComponent<Crewmates>() : null;
    public Crewmates Instantiate(string crewName, Vector3 position, Quaternion rotation)
        => crew.TryGetValue(crewName, out var crewmates) ? Instantiate(crewmates.gameObject, position, rotation).GetComponent<Crewmates>() : null;
    public Crewmates Instantiate(string crewName, Vector3 position, Quaternion rotation, Transform parent)
        => crew.TryGetValue(crewName, out var crewmates) ? Instantiate(crewmates.gameObject, position, rotation, parent).GetComponent<Crewmates>() : null;
}
