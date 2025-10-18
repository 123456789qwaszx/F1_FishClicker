using UnityEngine;

public class StringNameSpace : MonoBehaviour
{
    public const string CurrencyGain = "��ȭ ȹ�淮";
    public const string ExtraChanceRate = "�ѹ���! Ȯ��";
    public const string FeverTriggerRate = "�ǹ� ������";
    public const string LotteryWinRate = "���� ��÷��";
    public const string LotteryDiscountRate = "���� ���η�";

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
}