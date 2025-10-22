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
        if (UpgradeType == UpgradeType.None)
            return;

        switch (UpgradeType)
        {
            case UpgradeType.CurrencyGain:
                Txt_UpgradeValue.text = $"{StringNameSpace.CurrencyGain}Lv.({UpgradeSystem.Instance.GetLevel(UpgradeType)})\n+{UpgradeSystem.Instance.GetStatValue(UpgradeType)}%";
                Txt_UpgradePrice.text = $"{UpgradeSystem.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.RareOrAboveChanceBonus:
                Txt_UpgradeValue.text = $"{StringNameSpace.ExtraChanceRate}Lv.({UpgradeSystem.Instance.GetLevel(UpgradeType)})\n+{UpgradeSystem.Instance.GetStatValue(UpgradeType)}%";
                Txt_UpgradePrice.text = $"{UpgradeSystem.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.FeverGaugeFillRateUp:
                Txt_UpgradeValue.text = $"{StringNameSpace.FeverTriggerRate}Lv.({UpgradeSystem.Instance.GetLevel(UpgradeType)})\n+{UpgradeSystem.Instance.GetStatValue(UpgradeType)}%";
                Txt_UpgradePrice.text = $"{UpgradeSystem.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.LotteryWinRate:
                Txt_UpgradeValue.text = $"{StringNameSpace.LotteryWinRate}Lv.({UpgradeSystem.Instance.GetLevel(UpgradeType)})\n+{UpgradeSystem.Instance.GetStatValue(UpgradeType)}%";
                Txt_UpgradePrice.text = $"{UpgradeSystem.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
            case UpgradeType.LotteryDiscountRate:
                Txt_UpgradeValue.text = $"{StringNameSpace.LotteryDiscountRate}Lv.({UpgradeSystem.Instance.GetLevel(UpgradeType)})\n+{UpgradeSystem.Instance.GetStatValue(UpgradeType)}%";
                Txt_UpgradePrice.text = $"{UpgradeSystem.Instance.GetUpgradeCost(UpgradeType)}G";
                break;
        }
    }
    
    public void OnSlotClicked()
    {
        UIManager.Instance.FindUI<UI_UpgradePanel>()?.OnSlotClicked(Index);
    }
}