using TMPro;
using UnityEngine;

public class DuelistUIController : MonoBehaviour
{
    [SerializeField] TMP_Text balanceMeterText;
    [SerializeField] TMP_Text energyMeterText;

    DuelistCharacterController duelistController;

    void UpdateMeterUI(StatePayload state)
    {
        balanceMeterText.text = state.Balance.ToString();
        energyMeterText.text = state.Energy.ToString();
    }

    private void Awake()
    {
        duelistController = GetComponentInParent<DuelistCharacterController>();
        duelistController.EvtServerStateProcessed.AddListener(UpdateMeterUI);
    }

    private void OnDestroy()
    {
        duelistController.EvtServerStateProcessed.RemoveListener(UpdateMeterUI);

    }
}
