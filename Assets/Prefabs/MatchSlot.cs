using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MatchSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Txt_Name;
    [SerializeField] TextMeshProUGUI Txt_MatchPower;
    [SerializeField] TextMeshProUGUI Txt_LockNameText;
    [SerializeField] TextMeshProUGUI Txt_UnlockCondition;
    [SerializeField] GameObject Obj_LockOverlay;
    [SerializeField] Button Btn_Select;

    // MatchInfo matchInfo;
    // Action<MatchInfo> onSelectCallback;
    //
    // void Awake()
    // {
    //     Btn_Select.onClick.AddListener(OnSelect);
    // }
    //
    // // 가능하다면 SelSlot 말고 이걸로 언락을 해제할 것
    // public void RefreshUnlockState()
    // {
    //     bool isUnlocked = GameManager.Instance.CurTeam.IsUnlocked(matchInfo.UnlockLevel, matchInfo);
    //
    //     Obj_LockOverlay.SetActive(!isUnlocked);
    //     Btn_Select.gameObject.SetActive(isUnlocked);
    // }
    //
    //
    // public void SetSlot(MatchInfo info, Team t, Action<MatchInfo> onSelect = null)
    // {
    //     matchInfo = info;
    //     onSelectCallback = onSelect;
    //
    //     Txt_Name.text = $"[{GetDifficultyString(info)}]";
    //     Txt_MatchPower.text = $"적 경기력 : ({info.MinMatchPower} ~ {info.MaxMatchPower})";
    //
    //     bool isUnlocked = GameManager.Instance.CurTeam.IsUnlocked(info.UnlockLevel, info);
    //     bool isAvailableDate = true ;
    //
    //     int month = Calendar.Instance.CurrentMonth.Month;
    //     if (isUnlocked)
    //     {
    //         switch (info.Level)
    //         {
    //             // 월간대회 해금 : 매월 4주 평일에만 해금
    //             case 2:
    //             case 6:
    //                 {
    //                     if (Calendar.Instance.WeekCount != 4 && Calendar.Instance.IsWeekEnd == false)
    //                         isUnlocked = false;
    //                     isAvailableDate = false;
    //                 }
    //                 break;
    //             // 결산대회 해금 : 4월, 8월에만 해금
    //             case 3:
    //             case 7:
    //                 if (Calendar.Instance.WeekCount != 4 && Calendar.Instance.IsWeekEnd == false || month != 4 && month != 8)
    //                     isUnlocked = false;
    //                 isAvailableDate = false;
    //                 break;
    //             // 월즈대회 해금 : 12월에만 해금
    //             case 4:
    //             case 8:
    //                 if (Calendar.Instance.WeekCount != 4 && Calendar.Instance.IsWeekEnd == false || month != 12)
    //                     isUnlocked = false;
    //                 isAvailableDate = false;
    //                 break;
    //         }
    //     }
    //
    //     Obj_LockOverlay.SetActive(!isUnlocked);
    //     Btn_Select.gameObject.SetActive(isUnlocked);
    //
    //     //bool IsDateAvailable = Calendar.Instance.CurrentMonth.Month = 0;
    //
    //
    //     if (isUnlocked)
    //     {
    //         Txt_UnlockCondition.gameObject.SetActive(false);
    //     }
    //     else if (!isUnlocked && !isAvailableDate)
    //     {
    //         Txt_UnlockCondition.gameObject.SetActive(true);
    //         Txt_LockNameText.text = "대회 개최 예정";
    //
    //         switch (info.Level)
    //         {
    //             case 8: 
    //             case 4:
    //                 Txt_UnlockCondition.text = $"[12월]";
    //                 break;
    //             case 7: 
    //             case 3:
    //                 Txt_UnlockCondition.text = $"[4월, 8월]";
    //                 break;
    //             case 6:
    //             case 2:
    //                 Txt_UnlockCondition.text = $"[매월 넷째 주]";
    //                 break;
    //         }
    //         
    //     }
    //     else
    //     {
    //         Txt_UnlockCondition.gameObject.SetActive(true);
    //         Txt_LockNameText.text = "잠금";
    //
    //         switch (info.UnlockLevel)
    //         {
    //             case 7:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_HardGame2} / 우승 {info.UnlockCount}회";
    //                 break;
    //             case 6:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_HardGame1}/ 우승 {info.UnlockCount}회";
    //                 break;
    //             case 5:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_HardGame0} / 우승 {info.UnlockCount}회";
    //                 break;
    //             case 4:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_NormalGame3} / 우승 {info.UnlockCount}회";
    //                 break;
    //             case 3:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_NormalGame2} / 우승 {info.UnlockCount}회";
    //                 break;
    //             case 2:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_NormalGame1} / 우승 {info.UnlockCount}회";
    //                 break;
    //             case 1:
    //                 Txt_UnlockCondition.text = $"언락 조건 : {StringNameSpace.Scehdule_NormalGame0} / 우승 {info.UnlockCount}회";
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
    //
    //
    // string GetDifficultyString(MatchInfo info)
    // {
    //     if (!info.IsHardMode)
    //     {
    //         switch (info.Level)
    //         {
    //             case 1: return StringNameSpace.Scehdule_NormalGame0;
    //             case 2: return StringNameSpace.Scehdule_NormalGame1;
    //             case 3: return StringNameSpace.Scehdule_NormalGame2;
    //             case 4: return StringNameSpace.Scehdule_NormalGame3;
    //             default: return "-";
    //         }
    //     }
    //     else
    //     {
    //         switch (info.Level)
    //         {
    //             case 5: return StringNameSpace.Scehdule_HardGame0;
    //             case 6: return StringNameSpace.Scehdule_HardGame1;
    //             case 7: return StringNameSpace.Scehdule_HardGame2;
    //             case 8: return StringNameSpace.Scehdule_HardGame3;
    //             default: return "-";
    //         }
    //     }
    // }
    //
    // void OnSelect()
    // {
    //     UIManager.Instance.FindUIScene<UI_Competition>().MatchInfo = matchInfo;
    //
    //     foreach (Gamer g in GameManager.Instance.CurTeam.MainGamers)
    //     {
    //         if (g == null)
    //         {
    //             UIManager.Instance.ShowPopupUI<UI_NotEnoughGamerPopup>();
    //
    //             Debug.Log("선수 5명 안됨");
    //             return;
    //         }
    //     }
    //     if (onSelectCallback != null)
    //         onSelectCallback.Invoke(matchInfo);
    //
    //     UIManager.Instance.FindUIPopup<UI_SchedulePreparationPopup>().ClosePopupUI();
    //
    //     SpawnManager.Instance.DespawnTeams();
    //     SpawnManager.Instance.SpawnTeams();
    // }
}