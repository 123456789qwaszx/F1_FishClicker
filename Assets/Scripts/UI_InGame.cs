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
        Txt_FishAmount
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
    }
    #endregion
    
    TextMeshProUGUI Txt_GoldGain;
    TextMeshProUGUI Txt_AutoGainPerSec;
    TextMeshProUGUI Txt_FishAmount;
    
    Button Btn_Phone;
    
    Image Img_Gem;
    
    GameObject Obj_AutoGainPerSec;
    GameObject Obj_GoldGain;
    GameObject Obj_Gem;
    GameObject Obj_FishAmount;
    
    
    protected override void Awake()
    {

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(Objects));
        
        
        Txt_GoldGain = GetText((int)Texts.Txt_GoldGain);
        Txt_AutoGainPerSec = GetText((int)Texts.Txt_AutoGainPerSec);
        Txt_FishAmount = GetText((int)Texts.Txt_FishAmount);
        
        Btn_Phone = GetButton((int)Buttons.Btn_Phone);
        
        Img_Gem = GetImage((int)Images.Img_Gem);

        Obj_GoldGain = GetObject((int)Objects.Obj_GoldGain);
        Obj_AutoGainPerSec = GetObject((int)Objects.Obj_AutoGainPerSec);
        Obj_Gem = GetObject((int)Objects.Obj_Gem);
        Obj_FishAmount = GetObject((int)Objects.Obj_FishAmount);
        
        
        Img_Gem.sprite = Resources.Load<Sprite>("UI_InGame/Img_Gem");
        Btn_Phone.image.sprite = Resources.Load<Sprite>("UI_InGame/Img_Phone");
        
        BindEvent(Btn_Phone.gameObject, OnShowPhone);
    }

    public void RefreshUI()
    {
        Txt_GoldGain.text = $"{GameManager.Instance.Money}G";
        Txt_FishAmount.text = $"{GameManager.Instance.fishCaughtCount}F";
    }
    
    public void OnShowPhone(PointerEventData _)
    {
        UIManager.Instance.CloseAllPopupUI();
        UIManager.Instance.ShowPopup<UI_Phone>(popup => { popup.UpdateSlots(); });
        UIManager.Instance.ShowPopup<UI_AppMenuPanel>(null, UIManager.Instance.FindUI<UI_Phone>()?.AppMenuPanelBox);
    }
    
}
