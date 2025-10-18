using UnityEngine;

public class UI_UpgradePanel : UI_Popup
{
    Transform SlotPanel;
    public UpgradeSlot[] slots = new UpgradeSlot[5];
    int _curIndex = -1;

    public void SetUp()
    {
        SlotPanel = ComponentHelper.TryFindChild(this, "SlotPanel");

        for (int i = 0; i < SlotPanel.childCount; i++)
        {
            GameObject go = SlotPanel.GetChild(i).gameObject;
            UpgradeSlot slot = go.GetComponent<UpgradeSlot>();
            slot.UpgradeType = (UpgradeType)i + 1;
            slot.Index = i;
            slot.SetupByUpgradeType();
            slots[i] = slot;
        }
    }

    public void UpdateSlots()
    {
        foreach (UpgradeSlot us in slots)
        {
            us.SetupByUpgradeType();
        }
    }

    public void OnSlotClicked(int slotIndex)
    {
        _curIndex = slotIndex;
        UpgradeManager.Instance.TryUpgrade(slots[_curIndex].UpgradeType);
        UpdateSlots();

        EventManager.Instance.TriggerEvent(EEventType.MoneyChanged);
    }
}