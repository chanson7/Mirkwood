using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PregameMenuHandler))]
public class PregameMenuHandlerEditor : Editor
{
    private PregameMenuHandler pregameMenuHandler;

    private void OnEnable()
    {
        pregameMenuHandler = target as PregameMenuHandler;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Connect to Local Server"))
        {

        }
    }
}
