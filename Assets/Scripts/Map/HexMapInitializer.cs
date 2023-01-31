using UnityEngine;
using Mirror;
using HexMapTools;

public class HexMapInitializer : NetworkBehaviour
{
    [SerializeField] HexMapDefinition mapDefinition;

    [Tooltip("In # of hexagons")]
    [SerializeField] int hexRadius;
    [SerializeField] HexGrid hexGrid;

    public override void OnStartServer()
    {
        SpawnMapObjects(hexGrid);

        base.OnStartServer();
    }

    [Server]
    void SpawnMapObjects(HexGrid hexGrid)
    {

        for (int x = -hexRadius; x < hexRadius; x++)
        {
            for (int y = -hexRadius; y < hexRadius; y++)
            {
                HexCoordinates coordinates = HexCoordinates.FromOffsetCoordinates(x, y);
                float hypotenuse = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));

                GameObject mapObject = Instantiate(mapDefinition.SelectObjectByDistanceFromOrigin(hypotenuse),
                                                   hexGrid.HexCalculator.HexToPosition(coordinates),
                                                   Quaternion.identity);

                NetworkServer.Spawn(mapObject);
            }

        }


        Debug.Log($"..Spawned Map Objects");

    }
}
