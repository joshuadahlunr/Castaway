using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     A script that detects when other game objects collide with the game object to which it is attached.
/// </summary>
/// <author>Joshua Dahl</author>
public class CollidingObjects : MonoBehaviour {
    /// <summary>
    ///     A set of all GameObjects that are currently colliding with this game object.
    /// </summary>
    public readonly HashSet<GameObject> objects = new();

    /// <summary>
    ///     Returns true if any GameObjects are currently colliding with this game object.
    /// </summary>
    public bool AnyColliding => objects.Count > 0;

#if UNITY_EDITOR
    /// <summary>
    ///     An array of all colliding GameObjects, only serialized when running in the Unity editor for debugging purposes.
    /// </summary>
    [SerializeField] private GameObject[] objectsDebug;
#endif

    /// <summary>
    ///     Called when a GameObject enters the trigger zone of this game object's collider.
    /// </summary>
    /// <param name="other">The Collider component of the other GameObject that collided with this game object.</param>
    private void OnTriggerEnter(Collider other) {
		// Add the colliding GameObject to the set
		objects.Add(other.gameObject);

#if UNITY_EDITOR
		// Serialize the set of colliding objects into an array for debugging purposes in the editor
		objectsDebug = objects.ToArray();
#endif
	}

    /// <summary>
    ///     Called when a GameObject exits the trigger zone of this game object's collider.
    /// </summary>
    /// <param name="other">The Collider component of the other GameObject that exited the trigger zone of this game object.</param>
    private void OnTriggerExit(Collider other) {
		// Remove the colliding GameObject from the set
		objects.Remove(other.gameObject);

#if UNITY_EDITOR
		// Serialize the set of colliding objects into an array for debugging purposes in the editor
		objectsDebug = objects.ToArray();
#endif
	}

    /// <summary>
    ///     Returns an array of all colliding GameObjects that have a component of type T.
    /// </summary>
    /// <typeparam name="T">The type of component to look for in the colliding GameObjects.</typeparam>
    /// <returns>An array of all colliding GameObjects that have a component of type T.</returns>
    public T[] GetCollidingObjectsOfType<T>() {
		return objects
			.Select(obj => obj.GetComponent<T>())
			.Where(component => component is not null)
			.ToArray();
	}
}