using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Phone : UI_Popup
{
    #region enum
    enum Texts
    {
    }


    enum Buttons
    {
        Btn_Home,
        Btn_Back,
        Btn_Icon00,
        Btn_Icon00_Pressed,
        Btn_Icon00_Normal,
        Btn_Icon01,
        Btn_Icon02,
        Btn_Icon02_Pressed,
        Btn_Icon02_Normal,
    }


    enum Images
    {
        Img_Phone,
        Img_SafeAreaBottom,
        Img_RecentAppsButton,
    }


    enum Objects
    {
    }

    #endregion

    Image Img_Phone;
    Image Img_SafeAreaBottom;
    Image Img_RecentAppsButton;
    Button Btn_Home;
    Button Btn_Back;
    Button Btn_Icon00;
    Button Btn_Icon00_Pressed;
    Button Btn_Icon00_Normal;
    Button Btn_Icon01;
    Button Btn_Icon02;
    Button Btn_Icon02_Pressed;
    Button Btn_Icon02_Normal;


    protected override void Awake()
    {
        base.Awake();

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(Objects));

        Img_Phone = GetImage((int)Images.Img_Phone);
        Img_SafeAreaBottom = GetImage((int)Images.Img_SafeAreaBottom);
        Img_RecentAppsButton = GetImage((int)Images.Img_RecentAppsButton);
        Btn_Home = GetButton((int)Buttons.Btn_Home);
        Btn_Back = GetButton((int)Buttons.Btn_Back);
        Btn_Icon00 = GetButton((int)Buttons.Btn_Icon00);
        Btn_Icon00_Pressed = GetButton((int)Buttons.Btn_Icon00_Pressed);
        Btn_Icon00_Normal = GetButton((int)Buttons.Btn_Icon00_Normal);
        Btn_Icon01 = GetButton((int)Buttons.Btn_Icon01);
        Btn_Icon02 = GetButton((int)Buttons.Btn_Icon02);
        Btn_Icon02_Pressed = GetButton((int)Buttons.Btn_Icon02_Pressed);
        Btn_Icon02_Normal = GetButton((int)Buttons.Btn_Icon02_Normal);
    }
    
    public void UpdateSlots()
    {
        
    }
}
