using UnityEngine.SceneManagement;
using UnityEngine;

public class AppInitializer : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
