using System.Collections.Generic;

[System.Serializable]
public class StageData
{
    public int stageId;                 // 맵 내 단계 번호 (0~9)
    public string stageName;            // 예: "깊은 심해 1"
    public int unlockRequirement;       // 이전 스테이지 클리어 ID
    public int rewardGold;              // 보상
    public List<FishData> availableFishes; // 등장하는 물고기
}