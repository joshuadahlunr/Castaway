using System;
using System.Collections;
using UnityEngine;

public class Notification : MonoBehaviour {
    public TMPro.TMP_Text text;
    private Vector3 targetPosition;
    public float lifetime;

    public void Start() {
        IEnumerator SlideIn() {
            yield return null;

            targetPosition = transform.position;

            float delta = 0;
            while (delta < 1) {
                delta += Time.deltaTime / .3f; // The divide by .3 means it will take that long to slide in
                yield return null;

                var pos = transform.position;
                pos.x = Mathf.Lerp(-100, targetPosition.x, delta);
                transform.position = pos;
            }
        }

        StartCoroutine(SlideIn());
    }

    public void Update() {
        lifetime -= Time.deltaTime;

        if(lifetime < 0)
            Destroy(gameObject);
    }
}
