using UnityEngine;
using TMPro;

public class PredictionDebugUIController : MonoBehaviour
{
    [SerializeField] TMP_Text _tickText;
    [SerializeField] TMP_Text _positionText;
    [SerializeField] TMP_Text _rotationText;
    [SerializeField] TMP_Text _lookDirectionText;
    [SerializeField] TMP_Text _velocityText;
    [SerializeField] TMP_Text _playerStateText;

    public void UpdateTextUI(StatePayload statePayload)
    {
        _tickText.text = "Tick: " + statePayload.Tick.ToString();
        _positionText.text = "Position: " + statePayload.Position.ToString();
        _rotationText.text = "Rotation: " + statePayload.Rotation.ToString();
        _lookDirectionText.text = "Look Direction: " + statePayload.LookDirection.ToString();
        _velocityText.text = "Velocity: " + statePayload.Velocity.ToString();
        _playerStateText.text = "Player State: " + statePayload.PlayerState.ToString();
    }
}
