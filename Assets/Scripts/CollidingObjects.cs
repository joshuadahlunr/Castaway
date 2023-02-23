using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollidingObjects : MonoBehaviour {
	public readonly HashSet<GameObject> objects = new();
	public bool AnyColliding => objects.Count > 0;

#if UNITY_EDITOR
	[SerializeField] GameObject[] objectsDebug;
#endif

	private void OnTriggerEnter(Collider other) {
		objects.Add(other.gameObject);
#if UNITY_EDITOR
		objectsDebug = objects.ToArray();
#endif
	}

	private void OnTriggerExit(Collider other) {
		objects.Remove(other.gameObject);
#if UNITY_EDITOR
		objectsDebug = objects.ToArray();
#endif
	}

	public T[] GetCollidingObjectsOfType<T>() {
		return objects.Select(obj => obj.GetComponent<T>()).Where(component => component is not null).ToArray();
	}
}