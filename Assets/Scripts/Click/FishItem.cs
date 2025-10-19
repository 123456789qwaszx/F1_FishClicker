using System;
using UnityEngine;

[Serializable]
public class FishItem
{
    // ===============================
    // 1. 기본 정보 (불변)
    // ===============================
    public FishData data; // 물고기 종류 정보 (ScriptableObject)
    
    // ===============================
    // 2. 인벤토리 상태
    // ===============================
    public int quantity;      // 소유 수량
    public float freshness;   // 신선도 (0~1)
    public bool isProcessed;  // 가공 여부
    
    // ===============================
    // 3. 생성자
    // ===============================
    public FishItem(FishData fishData, int qty = 1, float freshness = 1f, bool processed = false)
    {
        this.data = fishData;
        this.quantity = qty;
        this.freshness = freshness;
        this.isProcessed = processed;
    }
    
    // ===============================
    // 4. 수량 증가/감소
    // ===============================
    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public bool RemoveQuantity(int amount)
    {
        if (amount > quantity)
            return false;

        quantity -= amount;
        return true;
    }

    // ===============================
    // 5. 상태 변경 메서드
    // ===============================
    public void Process()
    {
        isProcessed = true;
    }

    public void SetFreshness(float value)
    {
        freshness = Mathf.Clamp01(value); // 0~1 제한
    }

    // ===============================
    // 6. 편리한 정보 출력
    // ===============================
    public override string ToString()
    {
        return $"{data.fishName} x{quantity} | Rarity: {data.rarity} | Freshness: {freshness:F2} | Processed: {isProcessed}";
    }
}
