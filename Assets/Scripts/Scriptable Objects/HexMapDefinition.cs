using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HexMapDefinition", menuName = "ScriptableObjects/HexMapDefinition", order = 1)]
public class HexMapDefinition : ScriptableObject
{

    [SerializeField] List<GameObject> environmentObjects = new List<GameObject>();

    public GameObject SelectObjectByDistanceFromOrigin(float tileDistanceFromOrigin)
    {
        return environmentObjects[0];
    }


}
