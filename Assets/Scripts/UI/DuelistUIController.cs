using TMPro;
using UnityEngine;

public class DuelistUIController : MonoBehaviour
{
    [SerializeField] TMP_Text balanceMeterText;
    [SerializeField] TMP_Text energyMeterText;

    DuelistCharacterController duelistController;

    void UpdateDuelistUI(StatePayload state)
    {
        balanceMeterText.text = state.Balance.ToString();
        energyMeterText.text = state.Energy.ToString();
    }

    private void Awake()
    {
        duelistController = GetComponentInParent<DuelistCharacterController>();
        duelistController.EvtServerStateProcessed.AddListener(UpdateDuelistUI);
    }

    private void OnDestroy()
    {
        duelistController.EvtServerStateProcessed.RemoveListener(UpdateDuelistUI);

    }
}
