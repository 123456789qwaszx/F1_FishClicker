using UnityEngine;

public class StringNameSpace : MonoBehaviour
{
    public const string CurrencyGain = "골드획득량";
    public const string ExtraChanceRate = "배율 증가";
    public const string FeverTriggerRate = "풍어 확률 증가";
    public const string LotteryWinRate = "젬 획득 확률 증가";
    public const string LotteryDiscountRate = "자동 배율 증가";

    public const string NPCDialogue00 = "���õ� ���� �緯 ���̳���?";
    public const string NPCDialogue01 = "������ ���� �տ���\n�ٷ� ������ �� �־��.";
    public const string NPCDialogue02 = "���� ���\n���������� �����!";
    public const string NPCDialogue03 = "������ �� �� �ʿ��ϼ���?";

    public static readonly string[] dialogues =
    {
        StringNameSpace.NPCDialogue00,
        StringNameSpace.NPCDialogue01,
        StringNameSpace.NPCDialogue02,
        StringNameSpace.NPCDialogue03
    };
    
    public string GetRandomDialogue()
    {
        int index = Random.Range(0, dialogues.Length);
        return dialogues[index];
    }
    
    public static class ResourcePaths
    {
        public const string MapDataPath = "MapDatabase";
        public const string UpgradeDataPath = "UpgradeDatabase";
        public const string FishDataPath = "FishDatabase";
        public const string PlayerProfile = "Player/Profile";
        // 필요하면 추가
    }
    
    public static class UpgradeIDs
    {
        public const string Aria = "Aria";
        public const string Ciel = "Ciel";
        // 필요한 만큼 추가
    }

}