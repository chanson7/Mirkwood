using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTimeManager : MonoBehaviour
{

    private float minTimeBetweenServerTicks { get; set; }

    void Start()
    {
        minTimeBetweenServerTicks = 1f / MirkwoodNetworkManager.singleton.serverTickRate;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
