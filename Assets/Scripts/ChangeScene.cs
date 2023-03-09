using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// </summary>
/// <author>Jared White</author>

public class ChangeScene : MonoBehaviour {

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}