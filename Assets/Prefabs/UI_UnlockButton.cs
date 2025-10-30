using System.Collections;
using TMPro;
using UnityEngine;

public class UI_UnlockButton : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _failMessageText;
    [SerializeField] UnlockableBase Owner;


    //public int RequiredMoney;
    public int RequiredLevel;

    Coroutine _failMessageCoroutine;
    public float FailMessageDuration = 1.5f;


    void Start()
    {
        if (_failMessageText != null)
            _failMessageText.gameObject.SetActive(false);
    }

    //
    public void TryUnlock()
    {
    //     if (Owner == null)
    //         return;
    //
    //     int curLevel = GameManager.Instance.CurTeam.LastMatchWin + 1;
    //
    //     if (curLevel < RequiredLevel)
    //     {
    //         //ShowFailMessage("레벨이 부족합니다!");
    //         return;
    //     }
    //
    //     // if (GameManager.Instance.Money < RequiredMoney)
    //     // {
    //     //     ShowFailMessage("돈이 부족합니다!");
    //     //     return;
    //     // }
    //
    //     //GameManager.Instance.Money -= RequiredMoney;
    //     Owner.SetUnlockedState(EUnlockedState.Unlocked);
    //
    //     EventManager.Instance.TriggerEvent(EEventType.GameStatusUpdated);
    }
    //
    //
    public void RefreshUI(int requiredLevel)
    {
    //     //RequiredMoney = requiredMoney;
    //     RequiredLevel = requiredLevel;
    //
    //     //_moneyText.text = $"해금 :{UIHelper.GetMoneyText(requiredMoney)}G";
    //
    //     int curLevel = GameManager.Instance.CurTeam.LastMatchWin + 1;
    //
    //     switch (requiredLevel)
    //     {
    //         case 2:
    //             if (curLevel >= RequiredLevel)
    //             {
    //                 _levelText.text = "해금가능";
    //                 break;
    //             }
    //             _levelText.text = $"[{requiredLevel}]레벨 필요({StringNameSpace.Scehdule_NormalGame1} 우승)".ToString();
    //             break;
    //         case 4:
    //             if (curLevel >= RequiredLevel)
    //             {
    //                 _levelText.text = "해금가능";
    //                 break;
    //             }
    //             _levelText.text = $"[{requiredLevel}]레벨 필요({StringNameSpace.Scehdule_NormalGame3} 우승)".ToString();
    //             break;
    //         case 5:
    //             if (curLevel >= RequiredLevel)
    //             {
    //                 _levelText.text = "해금가능";
    //                 break;
    //             }
    //             _levelText.text = $"[{requiredLevel}]레벨 필요({StringNameSpace.Scehdule_HardGame0} 우승)".ToString();
    //             break;
    //         case 6:
    //             if (curLevel >= RequiredLevel)
    //             {
    //                 _levelText.text = "해금가능";
    //                 break;
    //             }
    //             _levelText.text = $"[{requiredLevel}]레벨 필요({StringNameSpace.Scehdule_HardGame1} 우승)".ToString();
    //             break;
    //         default:
    //             break;
    //     }
    }
}