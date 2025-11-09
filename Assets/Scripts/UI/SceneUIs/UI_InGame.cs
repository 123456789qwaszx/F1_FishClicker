using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    #region enum

    enum Texts
    {
        Txt_GoldGain,
        Txt_AutoGainPerSec,
        Txt_FishAmount0,
        Txt_FishAmount1,
        Txt_FishAmount2,
        Txt_FishAmount3,
        Txt_CurrentMap,
        Txt_CurrentStage,
    }

    enum Buttons
    {
        Btn_Phone,
        Btn_PrevStage,
        Btn_NextStage,
    }

    enum Images
    {
        Img_Gem,
    }
    
    enum Objects
    {
        Obj_GoldGain,
        Obj_AutoGainPerSec,
        Obj_Gem,
        Obj_FishAmount,
        Obj_CurrentRegion,
    }
    #endregion
    
    TextMeshProUGUI Txt_GoldGain;
    TextMeshProUGUI Txt_AutoGainPerSec;
    TextMeshProUGUI Txt_FishAmount0;
    TextMeshProUGUI Txt_FishAmount1;
    TextMeshProUGUI Txt_FishAmount2;
    TextMeshProUGUI Txt_FishAmount3;
    TextMeshProUGUI Txt_CurrentMap;
    TextMeshProUGUI Txt_CurrentStage;

    
    Button Btn_Phone;
    Button Btn_PrevStage;
    Button Btn_NextStage;
    
    Image Img_Gem;
    
    GameObject Obj_AutoGainPerSec;
    GameObject Obj_GoldGain;
    GameObject Obj_Gem;
    GameObject Obj_FishAmount;
    GameObject Obj_CurrentRegion;
    
    
    protected override void Awake()
    {
        gameObject.transform.localPosition = Vector3.zero;
        
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(Objects));
        
        
        Txt_GoldGain = GetText((int)Texts.Txt_GoldGain);
        Txt_AutoGainPerSec = GetText((int)Texts.Txt_AutoGainPerSec);
        Txt_FishAmount0 = GetText((int)Texts.Txt_FishAmount0);
        Txt_FishAmount1 = GetText((int)Texts.Txt_FishAmount1);
        Txt_FishAmount2 = GetText((int)Texts.Txt_FishAmount2);
        Txt_FishAmount3 = GetText((int)Texts.Txt_FishAmount3);
        Txt_CurrentMap = GetText((int)Texts.Txt_CurrentMap);
        Txt_CurrentStage = GetText((int)Texts.Txt_CurrentStage);
        
        Btn_Phone = GetButton((int)Buttons.Btn_Phone);
        Btn_PrevStage = GetButton((int)Buttons.Btn_PrevStage);
        Btn_NextStage = GetButton((int)Buttons.Btn_NextStage);
        
        Img_Gem = GetImage((int)Images.Img_Gem);

        Obj_GoldGain = GetObject((int)Objects.Obj_GoldGain);
        Obj_AutoGainPerSec = GetObject((int)Objects.Obj_AutoGainPerSec);
        Obj_Gem = GetObject((int)Objects.Obj_Gem);
        Obj_FishAmount = GetObject((int)Objects.Obj_FishAmount);
        Obj_CurrentRegion = GetObject((int)Objects.Obj_CurrentRegion);
        
        
        Img_Gem.sprite = Resources.Load<Sprite>("UI_InGame/Img_Gem");
        Btn_Phone.image.sprite = Resources.Load<Sprite>("UI_InGame/Img_Phone");
        
        BindEvent(Btn_Phone.gameObject, OnShowPhone);
    }

    public void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.MoneyChanged, UpdateHUD);
        EventManager.Instance.AddEvent(EEventType.OnMapChanged, UpdateRegionUI);
        EventManager.Instance.AddEvent(EEventType.OnStageChanged, UpdateRegionUI);
    }
    
    public void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.MoneyChanged, UpdateHUD);
        EventManager.Instance.RemoveEvent(EEventType.OnMapChanged, UpdateRegionUI);
        EventManager.Instance.RemoveEvent(EEventType.OnStageChanged, UpdateRegionUI);
    }

    public void UpdateHUD()
    {
        Txt_GoldGain.text = $"{GameManager.Instance.Money}G";
    }
    
    public void UpdateRegionUI()
    {
        var fishes = FishingManager.Instance.GetAvailableFishes();
        
        Txt_FishAmount0.text = (fishes.Count > 0) ? fishes[0].fishName : "";
        Txt_FishAmount1.text = (fishes.Count > 1) ? fishes[1].fishName : "";
        Txt_FishAmount2.text = (fishes.Count > 2) ? fishes[2].fishName : "";
        Txt_FishAmount3.text = (fishes.Count > 3) ? fishes[3].fishName : "";
        //Txt_AutoGainPerSec.text = $"1 / {1 - UpgradeManager.Instance.GetUpgradeAmount(StringNameSpace.UpgradeIDs.Aria)}Sec";
        Txt_CurrentMap.text =  $"현재 지역  {MapManager.Instance.CurrentMapData.region}";
        Txt_CurrentStage.text =  $"[{MapManager.Instance.CurrentStageData.StageId}]스테이지";
    }
    
    
    public void OnChangeNextMap()
    {
        MapManager.Instance.MoveToNextStage();
    }
    
    public void OnChangePrevMap()
    {
        MapManager.Instance.MoveToPrevStage();
        EventManager.Instance.TriggerEvent(EEventType.OnMapChanged);
    }
    
    public void OnShowPhone(PointerEventData _)
    {
        UIManager.Instance.CloseAllPopups();
        UIManager.Instance.ShowPopup<UI_Phone>(popup => { popup.UpdateSlots(); });
        UIManager.Instance.ShowPopup<UI_AppMenuPanel>(null, UIManager.Instance.GetUI<UI_Phone>()?.AppMenuPanelBox);
    }
    
}
