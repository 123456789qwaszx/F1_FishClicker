using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Txt_UpgradeValue;
    [SerializeField] TextMeshProUGUI Txt_UpgradePrice;

    public UpgradeType UpgradeType;
    public int Index;


    public void SetupByUpgradeType()
    {
        switch (UpgradeType)
        {
            case UpgradeType.Aria:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}()";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Ciel:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Reina:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Noel:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Lumia:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Kei:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.Mio:
                Txt_UpgradeValue.text = $"{UpgradeType}Lv.({GameManager.Instance.GetUpgradeLevel(UpgradeType)})\n+{GameManager.Instance.GetUpgradeAmount(UpgradeType)}";
                Txt_UpgradePrice.text = $"{GameManager.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
        }
    }
    
    public void OnSlotClicked()
    {
        UIManager.Instance.FindUI<UI_UpgradePanel>()?.OnSlotClicked(Index);
    }
}