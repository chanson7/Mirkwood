using UnityEngine;
using TMPro;

public class PlayerUIController : MonoBehaviour
{

    [SerializeField] TMP_Text balanceText;
    [SerializeField] TMP_Text energyText;

    public void SetBalanceText(int balanceValue)
    {
        balanceText.SetText(balanceValue.ToString());
    }

    public void SetEnergyText(int energyValue)
    {
        energyText.SetText(energyValue.ToString());
    }

}
