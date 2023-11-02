using UnityEngine;

[CreateAssetMenu(fileName = "App Manager", menuName = "ScriptableObjects/App Manager", order = 1)]
public class ApplicationManager : ScriptableObject {
	
	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
