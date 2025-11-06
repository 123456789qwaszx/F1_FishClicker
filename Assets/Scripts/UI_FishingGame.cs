using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FishingGame : UI_Popup
{
    [Header("Fish UI")]
    public Image Img_Fish;
    public TextMeshProUGUI Txt_FishName;
    public Slider Slider_FishHP;
    public TextMeshProUGUI Txt_FishHP;
    //public TextMeshProUGUI Txt_RewardPreview;

    void OnEnable()
    {
        // EventManager.Instance.AddEvent(EEventType.OnNewFishSpawn, InitFishUI);
        EventManager.Instance.AddEvent(EEventType.OnAttackFish, UpdateEnemyHpUI);
        // EventManager.Instance.AddEvent(EEventType.OnFishDefeated, OnFishDefeated);

        // 초기 물고기 스폰
        //FishManager.Instance.SpawnNewFish();
    }

    void OnDisable()
    {
        // EventManager.Instance.RemoveEvent(EEventType.OnNewFishSpawn, InitFishUI);
        EventManager.Instance.RemoveEvent(EEventType.OnAttackFish, RefreshFishUI);
        // EventManager.Instance.RemoveEvent(EEventType.OnFishDefeated, OnFishDefeated);
    }

    public void RefreshFishUI()
    {
        FishData fish = FishingManager.Instance.Controller.CurFish;
        Img_Fish.sprite = Resources.Load<Sprite>("Trash/Char/Img_Juno000"); //fish.fishSprite;
        Txt_FishName.text = fish.fishName;
        // Txt_RewardPreview.text = $"{fish.reward} G";
        
        double curHp = FishingManager.Instance.Controller.CurFishHp;
        double maxHp = FishingManager.Instance.Controller.CurFishMaxHp;
        
        float ratio = (float)(curHp / maxHp);
        
        Slider_FishHP.maxValue = 1;
        Slider_FishHP.value = ratio;
        Txt_FishHP.text = $"{curHp}/{maxHp}";
    }

    public void OnClickFish()
    {
        FishingManager.Instance.Controller.AttackCurrentFish();
    }

    void UpdateEnemyHpUI()
    {
        double curHp = FishingManager.Instance.Controller.CurFishHp;
        double maxHp = FishingManager.Instance.Controller.CurFishMaxHp;
        float ratio = (float)(curHp / maxHp);

        Slider_FishHP.value = ratio;
        Txt_FishHP.text = $"{curHp}/{maxHp}";
        
        
        FishData fish = FishingManager.Instance.Controller.CurFish;
        Txt_FishName.text = fish.fishName;
    }

    void OnFishDefeated()
    {
        // 애니메이션이나 효과 넣고 싶으면 여기서
        Debug.Log("UI: 물고기 잡음, 다음 물고기 스폰 예정");
    }
}