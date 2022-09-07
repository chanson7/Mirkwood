using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MirkwoodNetworkManager))]
public class NetworkManagerEditor : Editor
{
    private MirkwoodNetworkManager networkManager;
    [SerializeField] PlayerSession playerSession;

    private void OnEnable()
    {
        networkManager = target as MirkwoodNetworkManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        playerSession = (PlayerSession)EditorGUILayout.ObjectField(playerSession, typeof(PlayerSession), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));

        if (GUILayout.Button("Connect to Local Server"))
        {
            networkManager.ConnectToPlayerSession(playerSession);
        }

    }

}
