using UnityEngine.SceneManagement;
using UnityEngine;

public class AppInitializer : MonoBehaviour
{

    public void LoadMainScene()
    {
        Debug.Log("Loading Main Scene");
        SceneManager.LoadScene("Main");
    }

}
