using UnityEngine;
using TMPro;

public class PredictionDebugUIController : MonoBehaviour
{
    [SerializeField] TMP_Text tickText;
    [SerializeField] TMP_Text positionText;
    [SerializeField] TMP_Text rotationText;
    [SerializeField] TMP_Text lookDirectionText;
    [SerializeField] TMP_Text velocityText;
    [SerializeField] TMP_Text playerStateText;
    [SerializeField] TMP_Text hitVectorText;
    [SerializeField] TMP_Text lastStateChangeTickText;

    public void UpdateTextUI(StatePayload statePayload)
    {
        tickText.text = "Tick: " + statePayload.Tick.ToString();
        positionText.text = "Position: " + statePayload.Position.ToString();
        rotationText.text = "Rotation: " + statePayload.Rotation.ToString();
        lookDirectionText.text = "Look Direction: " + statePayload.LookDirection.ToString();
        velocityText.text = "Velocity: " + statePayload.Velocity.ToString();
        playerStateText.text = "Player State: " + statePayload.PlayerState.ToString();
        hitVectorText.text = "Hit Vector: " + statePayload.HitVector.ToString();
        lastStateChangeTickText.text = "Last State Change Tick: " + statePayload.LastStateChangeTick.ToString();
    }
}
