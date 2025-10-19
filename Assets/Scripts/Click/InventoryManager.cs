using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<FishData, FishItem> inventory = new Dictionary<FishData, FishItem>();

    
    public void AddFish(FishData fish, int quantity = 1)
    {
        if (inventory.ContainsKey(fish))
        {
            inventory[fish].AddQuantity(quantity);
        }
        else
        {
            inventory[fish] = new FishItem(fish, quantity);
        }

        Debug.Log($"획득: {inventory[fish]}");
    }

    // ===============================
    // 5. 물고기 제거 (판매/가공)
    // ===============================
    public bool RemoveFish(FishData fish, int quantity = 1)
    {
        if (!inventory.ContainsKey(fish) || inventory[fish].quantity < quantity)
            return false;

        inventory[fish].RemoveQuantity(quantity);

        if (inventory[fish].quantity <= 0)
            inventory.Remove(fish);

        return true;
    }

    // ===============================
    // 6. 전체 인벤토리 반환
    // ===============================
    public List<FishItem> GetAllItems()
    {
        return new List<FishItem>(inventory.Values);
    }

    // ===============================
    // 7. 특정 물고기 수량 조회
    // ===============================
    public int GetQuantity(FishData fish)
    {
        return inventory.ContainsKey(fish) ? inventory[fish].quantity : 0;
    }

    // ===============================
    // 8. 디버그용 전체 출력
    // ===============================
    public void PrintAllItems()
    {
        Debug.Log("=== 인벤토리 ===");
        foreach (var item in inventory.Values)
        {
            Debug.Log(item);
        }
    }
}
