using UnityEngine;
using System.Collections.Generic;

public class UI_UpgradePanel : UI_Popup
{
    [SerializeField] private Transform SlotPanel;        // Slot을 담을 부모
    [SerializeField] private GameObject slotPrefab;      // UpgradeSlot prefab

    private List<UpgradeSlot> slots = new List<UpgradeSlot>();

    private int _curIndex = -1;

    
    public void SetUp()
    {
        ClearSlots();
        
        List<UpgradeData> upgrades = GameManager.Instance.GetAllUpgradeData();

        for (int i = 0; i < upgrades.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, SlotPanel);
            UpgradeSlot slot = go.GetComponent<UpgradeSlot>();
            slot.SetUpgradeData(upgrades[i]);
            slot.Index = i;

            slots.Add(slot);
        }
    }

    
    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        slots.Clear();
    }

    
    public void UpdateSlots()
    {
        foreach (UpgradeSlot us in slots)
        {
            us.RefreshUI();
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