using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour {
	public void Continue() {
		SceneManager.LoadScene("MainMenu");
	}
}