using UnityEngine;

public class Notification : MonoBehaviour {
    public TMPro.TMP_Text text;
    public float lifetime;

    public void Update() {
        lifetime -= Time.deltaTime;

        if(lifetime < 0)
            Destroy(gameObject);
    }
}
