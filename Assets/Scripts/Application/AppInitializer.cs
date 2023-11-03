using UnityEngine.SceneManagement;
using UnityEngine;

//this class is meant to get the app going correctly.  Does not persist
public class AppInitializer : MonoBehaviour
{

    bool _steamInitialized = false;

    public bool SteamInitialized { set 
        { 
            _steamInitialized = value;
            AppReady();
        } }

    void AppReady()
    {
        if (_steamInitialized)
            LoadMainScene();
    }

    void LoadMainScene()
    {
        Debug.Log("Loading Main Scene");
        SceneManager.LoadScene("Main");
    }

}
