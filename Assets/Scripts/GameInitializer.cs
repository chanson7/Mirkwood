using UnityEngine.SceneManagement;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
