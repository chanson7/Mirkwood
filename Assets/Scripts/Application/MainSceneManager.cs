using UnityEngine;

public class MainSceneManager : MonoBehaviour {

    private void Awake()
    {
		Cursor.lockState = CursorLockMode.None;
    }

    public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

}
