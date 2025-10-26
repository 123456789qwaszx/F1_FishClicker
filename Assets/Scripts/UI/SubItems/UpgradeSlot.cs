using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Txt_UpgradeValue;
    [SerializeField] TextMeshProUGUI Txt_UpgradePrice;

    public UpgradeType UpgradeType;
    public int Index;

    private UpgradeData _upgradeData;

    
    public void SetUpgradeData(UpgradeData data)
    {
        if (data == null) return;

        _upgradeData = data;
        UpgradeType = data.statType;
        RefreshUI();
    }

    
    public void RefreshUI()
    {
        if (_upgradeData == null) return;

        long upgradeAmount = _upgradeData.GetCurStatValue();
        long upgradeCost = _upgradeData.GetUpgradeCost();

        Txt_UpgradeValue.text = $"{UpgradeType} Lv.({_upgradeData.level})\n+{upgradeAmount} ({_upgradeData.effectType})";
        Txt_UpgradePrice.text = $"{upgradeCost}G";
    }

    
    public void OnSlotClicked()
    {
        UIManager.Instance.FindUI<UI_UpgradePanel>()?.OnSlotClicked(Index);
    }
}