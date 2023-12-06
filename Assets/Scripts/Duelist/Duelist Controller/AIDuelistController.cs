using Mirror;
using UnityEngine;

public class AIDuelistController : DuelistCharacterController
{
    public override void Tick()
    {
        if (isServerOnly)
        {
            RecordInput();
            HandleTickOnServer();
        }

        else if (isServer) HandleTickOnHost();
        else if (isClient) HandleTickOnClient();
    }

    [Server]
    void RecordInput()
    {
        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);

        foreach (DuelistControllerModule module in controllerModules)
        {
            if (module is IDuelistInputRecorder inputRecorder)
            {
                inputRecorder.RecordInput(ref inputPayload);
            }
        }

        inputQueue.Enqueue(inputPayload);
    }
}
