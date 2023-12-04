using UnityEngine;

public class AIDuelistBrain : MonoBehaviour
{
    PredictedPlayerMovement predictedMovement;
    PredictedPlayerCursorRotation predictedRotation;
    PredictedPlayerDodge predictedDodge;
    PredictedPlayerBlock predictedBlock;
    PredictedPlayerMeleeAttack predictedMeleeAttack;

    private void Awake()
    {
        predictedMovement = GetComponent<PredictedPlayerMovement>();
        predictedRotation = GetComponent<PredictedPlayerCursorRotation>();
        predictedDodge = GetComponent<PredictedPlayerDodge>();
        predictedBlock = GetComponent<PredictedPlayerBlock>();
        predictedMeleeAttack = GetComponent<PredictedPlayerMeleeAttack>();
    }

}
