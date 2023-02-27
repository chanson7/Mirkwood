using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedTransform))]
public abstract class PredictedTickProcessor : NetworkBehaviour
{

    private void Start()
    {
        gameObject.GetComponent<PredictedTransform>().RegisterTickProcessor(this);
    }

    public abstract InputPayload GatherInput(InputPayload inputPayload); //whatever is driving the movement should be here
    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload); //process the movement with the given input

}
