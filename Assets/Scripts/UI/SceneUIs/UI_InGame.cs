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
        Txt_CurrentStage,
    }

    enum Buttons
    {
        Btn_Phone,
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
        Obj_CurrentStage,
    }
    #endregion
    
    TextMeshProUGUI Txt_GoldGain;
    TextMeshProUGUI Txt_AutoGainPerSec;
    TextMeshProUGUI Txt_FishAmount0;
    TextMeshProUGUI Txt_FishAmount1;
    TextMeshProUGUI Txt_FishAmount2;
    TextMeshProUGUI Txt_FishAmount3;
    TextMeshProUGUI Txt_CurrentStage;

    
    Button Btn_Phone;
    
    Image Img_Gem;
    
    GameObject Obj_AutoGainPerSec;
    GameObject Obj_GoldGain;
    GameObject Obj_Gem;
    GameObject Obj_FishAmount;
    GameObject Obj_CurrentStage;
    
    
    protected override void Awake()
    {

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
        Txt_CurrentStage = GetText((int)Texts.Txt_CurrentStage);
        
        Btn_Phone = GetButton((int)Buttons.Btn_Phone);
        
        Img_Gem = GetImage((int)Images.Img_Gem);

        Obj_GoldGain = GetObject((int)Objects.Obj_GoldGain);
        Obj_AutoGainPerSec = GetObject((int)Objects.Obj_AutoGainPerSec);
        Obj_Gem = GetObject((int)Objects.Obj_Gem);
        Obj_FishAmount = GetObject((int)Objects.Obj_FishAmount);
        Obj_CurrentStage = GetObject((int)Objects.Obj_CurrentStage);
        
        
        Img_Gem.sprite = Resources.Load<Sprite>("UI_InGame/Img_Gem");
        Btn_Phone.image.sprite = Resources.Load<Sprite>("UI_InGame/Img_Phone");
        
        BindEvent(Btn_Phone.gameObject, OnShowPhone);
    }

    public void OnEnable()
    {
        EventManager.Instance.AddEvent(EEventType.MoneyChanged, RefreshUI);
    }
    
    public void OnDisable()
    {
        EventManager.Instance.RemoveEvent(EEventType.MoneyChanged, RefreshUI);
    }

    public void RefreshUI()
    {
        var fishes = MapManager.Instance.GetFishForMap();
        
        Txt_GoldGain.text = $"{GameManager.Instance.Money}G";
        Txt_FishAmount0.text = (fishes.Count > 0) ? fishes[0].fishName : "";
        Txt_FishAmount1.text = (fishes.Count > 1) ? fishes[1].fishName : "";
        Txt_FishAmount2.text = (fishes.Count > 2) ? fishes[2].fishName : "";
        Txt_FishAmount3.text = (fishes.Count > 3) ? fishes[3].fishName : "";
        Txt_AutoGainPerSec.text = $"1 / {1 - UpgradeSystem.Instance.GetStatValue(UpgradeType.Ciel)}Sec";
        Txt_CurrentStage.text =  $"현재 지역  {MapManager.Instance.GetCurrentMap().region}";
    }
    
    public void OnShowPhone(PointerEventData _)
    {
        UIManager.Instance.CloseAllPopupUI();
        UIManager.Instance.ShowPopup<UI_Phone>(popup => { popup.UpdateSlots(); });
        UIManager.Instance.ShowPopup<UI_AppMenuPanel>(null, UIManager.Instance.FindUI<UI_Phone>()?.AppMenuPanelBox);
    }
    
}
