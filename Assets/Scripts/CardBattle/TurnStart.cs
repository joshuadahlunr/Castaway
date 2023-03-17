using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TurnStart : MonoBehaviour {
    private Vector3 targetPosition;
    private Image image;
    void Start() {
        image = GetComponent<Image>();
        image.enabled = false;
        targetPosition = transform.position;

        IEnumerator SlideIn() {
            float delta = 0;
            while (delta < 1) {
                delta += Time.deltaTime / .3f; // The divide by .3 means it will take that long to slide in
                yield return null;

                var pos = transform.position;
                pos.x = Mathf.Lerp(-100, targetPosition.x, delta);
                transform.position = pos;
                image.enabled = true;
            }
        }

        StartCoroutine(SlideIn());

        IEnumerator FadeOut() {
            yield return new WaitForSeconds(2);

            float delta = 0;
            while (delta < 1) {
                delta += Time.deltaTime / .3f;
                yield return null;

                var c = image.color;
                c.a = 1 - delta;
                image.color = c;
            }
            Destroy(gameObject);
        }

        StartCoroutine(FadeOut());
    }
}
