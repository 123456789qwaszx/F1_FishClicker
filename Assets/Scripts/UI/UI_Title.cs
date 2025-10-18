using TMPro;
using UnityEngine;
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

    }
}
