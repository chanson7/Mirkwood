using UnityEngine;
using TMPro;

public class PlayerInterface : MonoBehaviour
{

    [SerializeField] GameObject interfaceObject;
    [SerializeField] TMP_Text balanceText;
    [SerializeField] TMP_Text energyText;

    private void Start()
    {
        interfaceObject.SetActive(true);
    }

    public void SetBalanceText(uint balanceValue)
    {
        balanceText.SetText(balanceValue.ToString());
    }

    public void SetEnergyText(uint energyValue)
    {
        energyText.SetText(energyValue.ToString());
    }

}
