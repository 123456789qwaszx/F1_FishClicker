using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Title : UI_Scene
{
    #region enum

    enum Texts
    {
        Txt_Start,
        Txt_End,
    }


    enum Buttons
    {
        Btn_Start,
        Btn_End,
    }


    enum Images
    {
        Img_TitleBG,
        Img_TitleText,
    }


    enum Objects
    {
    }

    #endregion

    Image Img_TitleBG;
    Button Btn_Start;
    TextMeshProUGUI Txt_Start;
    Button Btn_End;
    TextMeshProUGUI Txt_End;
    Image Img_TitleText;


    protected override void Awake()
    {
        base.Awake();

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(Objects));

        Img_TitleBG = GetImage((int)Images.Img_TitleBG);
        Btn_Start = GetButton((int)Buttons.Btn_Start);
        Txt_Start = GetText((int)Texts.Txt_Start);
        Btn_End = GetButton((int)Buttons.Btn_End);
        Txt_End = GetText((int)Texts.Txt_End);
        Img_TitleText = GetImage((int)Images.Img_TitleText);


        BindEvent(Btn_Start.gameObject, Btn_StartClick);
        BindEvent(Btn_End.gameObject, Btn_EndClick);
    }


    void Btn_StartClick(PointerEventData _)
    {
        UIManager.Instance.ChangeSceneUI<UI_InGame>(popup => { popup.UpdateMapUI(); });
        FishingManager.Instance.SpawnNewFish();
        UIManager.Instance.ShowPopup<UI_FishingGame>(popup => { popup.RefreshFishUI(); });
        
        
        //EventManager.Instance.Raise(UIEventType.ChangeScene, "UI_InGame");
    }

    void Btn_EndClick(PointerEventData _)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}