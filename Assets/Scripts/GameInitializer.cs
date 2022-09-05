using UnityEngine.SceneManagement;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("..Initializing Client");
        SceneManager.LoadScene("MainMenu");
    }
}
