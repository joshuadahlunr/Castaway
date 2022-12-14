using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Is this necessary?
public class ChangeScene : MonoBehaviour {

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}