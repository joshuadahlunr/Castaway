using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// @author: Jared White
/// </summary>

public class ChangeScene : MonoBehaviour {

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}